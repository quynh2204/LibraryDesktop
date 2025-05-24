# How to Access the Local Web Server

## Automatic Method (From Application)
1. Start the Library application
2. Click on the "Recharge" button in the main interface
3. Enter the recharge amount and generate a QR code
4. Click on the "Open payment in browser" link that appears

## Manual Method (Direct Browser Access)
1. Make sure the Library application is running
2. Open your web browser
3. Navigate to: http://localhost:8080
4. For specific payment page: http://localhost:8080/payment?token=YOUR_TOKEN
   - Replace YOUR_TOKEN with the actual payment token

## Testing Default User
For testing purposes, you can use the default user credentials:
- Username: demo_user
- Password: demo_hash

## Exchange Rate Information
- 1,000 VND = 1 Coin
- Minimum recharge: 10,000 VND (10 Coins)
- Maximum recharge: 1,000,000 VND (1,000 Coins)