// Payment confirmation JavaScript module
class PaymentConfirmation {    constructor() {
        this.paymentToken = null;
        this.paymentAmount = null;
        this.userId = null;
        // Use current hostname/IP instead of hardcoded IP for API server
        // Handle cases where hostname might be empty (file:// protocol)
        const hostname = window.location.hostname || 'localhost';
        this.apiBaseUrl = `http://${hostname}:5000`; // LibraryDesktop API server
        console.log('API Base URL:', this.apiBaseUrl);
        this.init();
    }
    init() {
        // Get URL parameters
        const urlParams = new URLSearchParams(window.location.search);
        
        // Check for embedded data parameter first
        const dataParam = urlParams.get('data');
        if (dataParam) {
            try {
                // Decode embedded payment data
                const paymentDataJson = atob(dataParam);
                const paymentData = JSON.parse(paymentDataJson);
                
                this.paymentToken = paymentData.token;
                this.paymentAmount = paymentData.amount;
                this.userId = paymentData.userId;
            } catch (error) {
                console.error('Failed to decode payment data:', error);
                this.showError('Invalid payment link');
                return;
            }
        } else {
            // Fallback to individual parameters
            this.paymentToken = urlParams.get('token');
            this.paymentAmount = urlParams.get('amount');
            this.userId = urlParams.get('userId');
        }

        // Initialize event listeners
        this.bindEvents();

        // Load payment details on page load
        if (!this.paymentToken) {
            this.showError('Invalid payment link');
            return;
        }
        
        this.loadPaymentDetails();
    }

    bindEvents() {
        // Confirm payment button
        const confirmBtn = document.getElementById('confirm-btn');
        if (confirmBtn) {
            confirmBtn.addEventListener('click', () => this.confirmPayment());
        }

        // Cancel button
        const cancelBtn = document.getElementById('cancel-btn');
        if (cancelBtn) {
            cancelBtn.addEventListener('click', () => this.cancelPayment());
        }

        // Handle browser navigation
        window.addEventListener('beforeunload', (e) => {
            if (this.paymentToken) {
                e.preventDefault();
                e.returnValue = 'Are you sure you want to leave? Your payment may not be completed.';
            }
        });

        // Auto-refresh payment status
        this.startStatusPolling();
    }    async loadPaymentDetails() {
        try {
            this.showLoading('Loading payment details...');

            // Check if have embedded data
            const urlParams = new URLSearchParams(window.location.search);
            const dataParam = urlParams.get('data');
            
            let apiUrl;
            if (dataParam) {
                // Use embedded data parameter
                apiUrl = `${this.apiBaseUrl}/api/payment?data=${dataParam}`;
            } else {
                // Fallback to token parameter
                apiUrl = `${this.apiBaseUrl}/api/payment?token=${this.paymentToken}`;
            }

            const response = await fetch(apiUrl);
            
            if (!response.ok) {
                throw new Error('Payment not found or expired');
            }

            const payment = await response.json();
            
            // Update UI with payment details
            this.updatePaymentDisplay(payment);
            this.hideLoading();
            
        } catch (error) {
            this.hideLoading();
            this.showError('Failed to load payment details: ' + error.message);
        }
    }updatePaymentDisplay(payment) {
        const amountEl = document.getElementById('amount');
        const descriptionEl = document.getElementById('description');
        const userInfoEl = document.getElementById('user-info');

        if (amountEl) {
            amountEl.textContent = `${payment.amount} VND`;
        }
        
        if (descriptionEl) {
            // Calculate coins (1000 VND = 1 coin)
            const coins = Math.floor(payment.amount / 1000);
            const enhancedDescription = payment.description || 'Account recharge';
            descriptionEl.textContent = `${enhancedDescription} (+${coins} coins)`;
        }
        
        if (userInfoEl) {
            userInfoEl.textContent = `User: ${payment.user || 'Unknown'}`;
        }
    }async confirmPayment() {
        const confirmBtn = document.getElementById('confirm-btn');
        const cancelBtn = document.getElementById('cancel-btn');

        try {
            // Disable buttons and show loading
            this.setButtonsEnabled(false);
            this.showLoading('Processing payment...');

            // Check if have embedded data
            const urlParams = new URLSearchParams(window.location.search);
            const dataParam = urlParams.get('data');
            
            let requestBody;
            if (dataParam) {
                // Use embedded payment data
                try {
                    const paymentDataJson = atob(dataParam);
                    const paymentData = JSON.parse(paymentDataJson);
                    requestBody = JSON.stringify({ paymentData: paymentData });
                } catch (error) {
                    throw new Error('Invalid payment data');
                }
            } else {
                // Fallback to token-based confirmation
                requestBody = JSON.stringify({
                    token: this.paymentToken,
                    userId: this.userId
                });
            }

            const response = await fetch(`${this.apiBaseUrl}/confirm-payment`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: requestBody
            });

            const result = await response.json();            if (result.success) {
                // Calculate coins for success message
                const coins = Math.floor(this.paymentAmount / 1000);
                this.showSuccess(`Payment completed successfully! +${coins} coins added to your account`);
                
                // Auto-close after 3 seconds
                setTimeout(() => {
                    this.closeWindow();
                }, 3000);
                
                // Clear token to prevent navigation warning
                this.paymentToken = null;
                
            } else {
                throw new Error(result.message || 'Payment failed');
            }

        } catch (error) {
            this.hideLoading();
            this.showError('Payment confirmation failed: ' + error.message);
            this.setButtonsEnabled(true);
        }
    }

    cancelPayment() {
        if (confirm('Are you sure you want to cancel this payment?')) {
            this.closeWindow();
        }
    }

    closeWindow() {
        // Try different methods to close the window
        if (window.close) {
            window.close();
        }
        
        // If window.close doesn't work (security restrictions), redirect
        setTimeout(() => {
            window.location.href = 'about:blank';
        }, 100);
    }    startStatusPolling() {
        // Add polling timeout to prevent infinite loops
        let pollCount = 0;
        const maxPolls = 60; // Maximum 5 minutes of polling (60 * 5 seconds)
        
        // Poll payment status every 5 seconds
        this.statusInterval = setInterval(async () => {
            pollCount++;
            
            // Stop polling after max attempts or if token is cleared
            if (!this.paymentToken || pollCount >= maxPolls) {
                clearInterval(this.statusInterval);
                if (pollCount >= maxPolls) {
                    console.log('Payment status polling timeout reached');
                    this.showError('Payment confirmation timeout. Please check your payment status manually.');
                }
                return;
            }

            try {
                // Check if have embedded data
                const urlParams = new URLSearchParams(window.location.search);
                const dataParam = urlParams.get('data');
                
                let apiUrl;
                if (dataParam) {
                    // Use embedded data parameter
                    apiUrl = `${this.apiBaseUrl}/api/payment?data=${dataParam}`;
                } else {
                    // Fallback to token parameter
                    apiUrl = `${this.apiBaseUrl}/api/payment?token=${this.paymentToken}`;
                }

                const response = await fetch(apiUrl);
                if (response.ok) {
                    const payment = await response.json();
                    console.log(`Payment status check ${pollCount}/${maxPolls}: ${payment.status}`);
                      if (payment.status === 'Completed') {
                        clearInterval(this.statusInterval);
                        // Calculate coins for success message
                        const coins = Math.floor(this.paymentAmount / 1000);
                        this.showSuccess(`Payment completed successfully! +${coins} coins added to your account`);
                        this.paymentToken = null;
                        setTimeout(() => this.closeWindow(), 2000);
                    }
                } else {
                    console.log(`Payment status check failed: ${response.status}`);
                }
            } catch (error) {
                // Log errors but continue polling for a few more attempts
                console.log(`Status poll failed (attempt ${pollCount}/${maxPolls}):`, error);
                
                // If we've had too many consecutive errors, stop polling
                if (pollCount > 10 && error.message.includes('fetch')) {
                    clearInterval(this.statusInterval);
                    this.showError('Unable to connect to payment server. Please check your connection.');
                }
            }
        }, 5000);
    }

    setButtonsEnabled(enabled) {
        const confirmBtn = document.getElementById('confirm-btn');
        const cancelBtn = document.getElementById('cancel-btn');
        
        if (confirmBtn) confirmBtn.disabled = !enabled;
        if (cancelBtn) cancelBtn.disabled = !enabled;
    }

    showLoading(message = 'Loading...') {
        const loadingEl = document.getElementById('loading');
        if (loadingEl) {
            loadingEl.style.display = 'block';
            const textEl = loadingEl.querySelector('p');
            if (textEl) textEl.textContent = message;
        }
    }

    hideLoading() {
        const loadingEl = document.getElementById('loading');
        if (loadingEl) {
            loadingEl.style.display = 'none';
        }
    }

    showSuccess(message) {
        const successEl = document.getElementById('success-message');
        if (successEl) {
            successEl.textContent = '✅ ' + message;
            successEl.style.display = 'block';
        }
        this.hideError();
        this.hideLoading();
        
        // Hide confirm button
        const confirmBtn = document.getElementById('confirm-btn');
        if (confirmBtn) confirmBtn.style.display = 'none';
    }

    showError(message) {
        const errorEl = document.getElementById('error-message');
        if (errorEl) {
            errorEl.textContent = '❌ ' + message;
            errorEl.style.display = 'block';
        }
        this.hideSuccess();
        this.hideLoading();
    }

    hideSuccess() {
        const successEl = document.getElementById('success-message');
        if (successEl) {
            successEl.style.display = 'none';
        }
    }

    hideError() {
        const errorEl = document.getElementById('error-message');
        if (errorEl) {
            errorEl.style.display = 'none';
        }
    }
}

// Initialize payment confirmation when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    window.paymentConfirmation = new PaymentConfirmation();
});

// Legacy functions for backward compatibility
function confirmPayment() {
    if (window.paymentConfirmation) {
        window.paymentConfirmation.confirmPayment();
    }
}

function loadPaymentDetails() {
    if (window.paymentConfirmation) {
        window.paymentConfirmation.loadPaymentDetails();
    }
}

function showSuccess() {
    if (window.paymentConfirmation) {
        // Calculate coins if have payment amount
        const amount = window.paymentConfirmation.paymentAmount;
        if (amount) {
            const coins = Math.floor(amount / 1000);
            window.paymentConfirmation.showSuccess(`Payment completed successfully! +${coins} coins added to your account`);
        } else {
            window.paymentConfirmation.showSuccess('Payment completed successfully!');
        }
    }
}

function showError(message) {
    if (window.paymentConfirmation) {
        window.paymentConfirmation.showError(message);
    }
}
