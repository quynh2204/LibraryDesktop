using LibraryDesktop.Data.Services;
using LibraryDesktop.Models;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryDesktop.View
{
    public partial class Account : UserControl
    {
        private IUserService _userService;
        private IAuthenticationService _authService;
        private User? _currentUser;
        private int _currentUserId;

        // Constructor mặc định cho Design Time
        public Account()
        {
            InitializeComponent();
        }

        // Constructor với dependency injection cho Runtime
        public Account(IUserService userService, IAuthenticationService authService) : this()
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        // Method để initialize services nếu sử dụng constructor mặc định
        public void Initialize(IUserService userService, IAuthenticationService authService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public async Task LoadUserDataAsync(int userId)
        {
            // Kiểm tra services trước khi sử dụng
            if (_userService == null || _authService == null)
            {
                throw new InvalidOperationException("Services have not been initialized. Call Initialize() or use the constructor with parameters.");
            }

            try
            {
                _currentUserId = userId;
                _currentUser = await _userService.GetUserByIdAsync(userId);

                if (_currentUser != null)
                {
                    await DisplayUserInfo();
                }
                else
                {
                    MessageBox.Show("Unable to load user information!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task DisplayUserInfo()
        {
            if (_currentUser == null) return;

            // Display basic info
            lblUserId.Text = _currentUser.UserId.ToString();
            txtUsername.Text = _currentUser.Username;
            txtEmail.Text = _currentUser.Email;
            lblRegistrationDate.Text = _currentUser.RegistrationDate.ToString("dd/MM/yyyy");

            // Load coins
            var coins = await _userService.GetUserCoinsAsync(_currentUser.UserId);
            lblCoins.Text = $"{coins:N0} xu";

            // Load avatar
            await LoadAvatar();
        }

        private async Task LoadAvatar()
        {
            if (_currentUser?.AvatarUrl != null && !string.IsNullOrEmpty(_currentUser.AvatarUrl))
            {
                try
                {
                    using (var client = new System.Net.Http.HttpClient())
                    {
                        var imageBytes = await client.GetByteArrayAsync(_currentUser.AvatarUrl);
                        using (var ms = new MemoryStream(imageBytes))
                        {
                            picAvatar.Image = Image.FromStream(ms);
                        }
                    }
                }
                catch
                {
                    // Use default avatar if loading fails
                    SetDefaultAvatar();
                }
            }
            else
            {
                SetDefaultAvatar();
            }
        }

        private void SetDefaultAvatar()
        {
            // Create a simple default avatar
            var bitmap = new Bitmap(100, 100);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.FillEllipse(Brushes.LightGray, 0, 0, 100, 100);
                g.DrawString(_currentUser?.Username?.Substring(0, 1).ToUpper() ?? "U",
                    new Font("Segoe UI", 24, FontStyle.Bold), Brushes.White, 35, 30);
            }
            picAvatar.Image = bitmap;
        }

        private async void btnSaveProfile_Click(object sender, EventArgs e)
        {
            if (_currentUser == null || _userService == null) return;

            try
            {
                // Validate email
                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show("Email cannot be empty!", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Update user info
                _currentUser.Email = txtEmail.Text.Trim();

                var success = await _userService.UpdateUserAsync(_currentUser);

                if (success)
                {
                    MessageBox.Show("Profile updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to update profile!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating profile: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnChangeAvatar_Click(object sender, EventArgs e)
        {
            if (_userService == null) return;

            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                openFileDialog.Title = "Select Avatar";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Load and display the selected image
                        var image = Image.FromFile(openFileDialog.FileName);
                        picAvatar.Image = image;

                        // In a real app, you would upload this to a server and get a URL
                        // For now, we'll just save the local path
                        var success = await _userService.UpdateAvatarUrlAsync(_currentUserId, openFileDialog.FileName);

                        if (success)
                        {
                            MessageBox.Show("Avatar updated successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to update avatar!", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading image: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (_authService == null) return;

            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(txtCurrentPassword.Text))
                {
                    MessageBox.Show("Please enter your current password!", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtNewPassword.Text))
                {
                    MessageBox.Show("Please enter a new password!", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtNewPassword.Text != txtConfirmNewPassword.Text)
                {
                    MessageBox.Show("New password and confirmation do not match!", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtNewPassword.Text.Length < 6)
                {
                    MessageBox.Show("The new password must be at least 6 characters long!", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Change password
                var success = await _authService.ChangePasswordAsync(_currentUserId,
                    txtCurrentPassword.Text, txtNewPassword.Text);

                if (success)
                {
                    MessageBox.Show("Password changed successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Clear password fields
                    txtCurrentPassword.Clear();
                    txtNewPassword.Clear();
                    txtConfirmNewPassword.Clear();
                }
                else
                {
                    MessageBox.Show("Incorrect current password or an error occurred!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error changing password: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}