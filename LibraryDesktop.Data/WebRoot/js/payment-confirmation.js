// Payment confirmation JavaScript module
class PaymentConfirmation {
    constructor() {
        this.paymentToken = null;
        this.paymentAmount = null;
        this.userId = null;
        this.apiBaseUrl = 'http://192.168.1.4:5500'; // LibraryDesktop API server (matches NetworkConfiguration.GetApiServerUrl())
        this.init();
    }

    init() {
        // Get URL parameters
        const urlParams = new URLSearchParams(window.location.search);
        this.paymentToken = urlParams.get('token');
        this.paymentAmount = urlParams.get('amount');
        this.userId = urlParams.get('userId');

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

            const response = await fetch(`${this.apiBaseUrl}/api/payment?token=${this.paymentToken}&amount=${this.paymentAmount}`);
            
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
    }

    updatePaymentDisplay(payment) {
        const amountEl = document.getElementById('amount');
        const descriptionEl = document.getElementById('description');
        const userInfoEl = document.getElementById('user-info');

        if (amountEl) {
            amountEl.textContent = `$${payment.amount}`;
        }
        
        if (descriptionEl) {
            descriptionEl.textContent = payment.description || 'Account recharge';
        }
        
        if (userInfoEl) {
            userInfoEl.textContent = `User: ${payment.user || 'Unknown'}`;
        }
    }

    async confirmPayment() {
        const confirmBtn = document.getElementById('confirm-btn');
        const cancelBtn = document.getElementById('cancel-btn');

        try {
            // Disable buttons and show loading
            this.setButtonsEnabled(false);
            this.showLoading('Processing payment...');            const response = await fetch(`${this.apiBaseUrl}/confirm-payment`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    token: this.paymentToken,
                    userId: this.userId
                })
            });

            const result = await response.json();

            if (result.success) {
                this.showSuccess('Payment completed successfully!');
                
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
    }

    startStatusPolling() {
        // Poll payment status every 5 seconds
        this.statusInterval = setInterval(async () => {
            if (!this.paymentToken) {
                clearInterval(this.statusInterval);
                return;
            }            try {
                const response = await fetch(`${this.apiBaseUrl}/api/payment?token=${this.paymentToken}`);
                if (response.ok) {
                    const payment = await response.json();
                    if (payment.status === 'Completed') {
                        clearInterval(this.statusInterval);
                        this.showSuccess('Payment completed successfully!');
                        this.paymentToken = null;
                        setTimeout(() => this.closeWindow(), 2000);
                    }
                }
            } catch (error) {
                // Silently fail - don't disturb user experience
                console.log('Status poll failed:', error);
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
        window.paymentConfirmation.showSuccess('Payment completed successfully!');
    }
}

function showError(message) {
    if (window.paymentConfirmation) {
        window.paymentConfirmation.showError(message);
    }
}
