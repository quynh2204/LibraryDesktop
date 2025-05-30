using LibraryDesktop.Data.Services;
using LibraryDesktop.Models;
using System;
using System.Diagnostics;
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

        private async void btnChangeAvatar_Click(object sender, EventArgs e)
        {
            if (_userService == null) return;

            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                openFileDialog.Title = "Select Avatar";
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Disable button và hiển thị loading
                    btnChangeAvatar.Enabled = false;
                    btnChangeAvatar.Text = "Uploading...";

                    try
                    {
                        // Lưu đường dẫn file
                        string selectedFile = openFileDialog.FileName;

                        // Chạy toàn bộ process trong background
                        await Task.Run(async () =>
                        {
                            await ProcessAvatarChangeCompletelyAsync(selectedFile);
                        });
                    }
                    catch (Exception ex)
                    {
                        // Show error trên UI thread
                        if (this.IsHandleCreated && !this.IsDisposed)
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                MessageBox.Show($"Error updating avatar: {ex.Message}", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }));
                        }
                    }
                    finally
                    {
                        // Re-enable button trên UI thread
                        if (this.IsHandleCreated && !this.IsDisposed)
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                btnChangeAvatar.Enabled = true;
                                btnChangeAvatar.Text = "Change Avatar";
                            }));
                        }
                    }
                }
            }
        }

        private async Task ProcessAvatarChangeCompletelyAsync(string sourceFilePath)
        {
            string destinationPath = null;
            string relativePath = null;

            try
            {
                // 1. Prepare paths
                string appPath = Application.StartupPath;
                string avatarDir = Path.Combine(appPath, "Avatars");

                if (!Directory.Exists(avatarDir))
                {
                    Directory.CreateDirectory(avatarDir);
                }

                // 2. Delete old avatar
                if (!string.IsNullOrEmpty(_currentUser?.AvatarUrl))
                {
                    string oldAvatarPath = Path.Combine(appPath, _currentUser.AvatarUrl);
                    if (File.Exists(oldAvatarPath))
                    {
                        try
                        {
                            File.Delete(oldAvatarPath);
                        }
                        catch
                        {
                            // Ignore deletion errors
                        }
                    }
                }

                // 3. Create new file path
                string fileExtension = Path.GetExtension(sourceFilePath);
                string fileName = $"avatar_{_currentUserId}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
                destinationPath = Path.Combine(avatarDir, fileName);
                relativePath = Path.Combine("Avatars", fileName).Replace("\\", "/");

                // 4. Copy file
                using (var source = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read))
                using (var destination = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
                {
                    await source.CopyToAsync(destination);
                }

                // 5. Process and resize image
                byte[] processedImageData = await ProcessImageAsync(destinationPath);

                // 6. Update database
                bool dbSuccess = await _userService.UpdateAvatarUrlAsync(_currentUserId, relativePath);

                if (!dbSuccess)
                {
                    // Rollback file if database update failed
                    if (File.Exists(destinationPath))
                    {
                        File.Delete(destinationPath);
                    }
                    throw new Exception("Failed to update database");
                }

                // 7. Update UI on main thread
                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            UpdateAvatarUI(processedImageData);

                            // Update current user object
                            if (_currentUser != null)
                            {
                                _currentUser.AvatarUrl = relativePath;
                            }

                            MessageBox.Show("Avatar updated successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error updating UI: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }));
                }
            }
            catch (Exception)
            {
                // Cleanup on error
                if (!string.IsNullOrEmpty(destinationPath) && File.Exists(destinationPath))
                {
                    try
                    {
                        File.Delete(destinationPath);
                    }
                    catch { }
                }
                throw;
            }
        }

        private async Task<byte[]> ProcessImageAsync(string imagePath)
        {
            return await Task.Run(() =>
            {
                // Đọc và process image hoàn toàn trong background
                byte[] originalData = File.ReadAllBytes(imagePath);

                using (var ms = new MemoryStream(originalData))
                using (var originalImage = Image.FromStream(ms))
                {
                    // Resize image
                    using (var resizedImage = ResizeImage(originalImage, 200, 200))
                    {
                        // Convert về byte array
                        using (var outputMs = new MemoryStream())
                        {
                            resizedImage.Save(outputMs, System.Drawing.Imaging.ImageFormat.Jpeg);
                            return outputMs.ToArray();
                        }
                    }
                }
            });
        }

        private void UpdateAvatarUI(byte[] imageData)
        {
            try
            {
                // Dispose old image
                if (picAvatar.Image != null)
                {
                    var oldImage = picAvatar.Image;
                    picAvatar.Image = null;
                    oldImage.Dispose();
                }

                // Set new image
                using (var ms = new MemoryStream(imageData))
                {
                    picAvatar.Image = new Bitmap(Image.FromStream(ms));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating avatar UI: {ex.Message}");
                SetDefaultAvatar();
            }
        }

        private Image ResizeImage(Image originalImage, int maxWidth, int maxHeight)
        {
            int newWidth = originalImage.Width;
            int newHeight = originalImage.Height;

            if (newWidth > maxWidth || newHeight > maxHeight)
            {
                double ratio = Math.Min((double)maxWidth / newWidth, (double)maxHeight / newHeight);
                newWidth = (int)(newWidth * ratio);
                newHeight = (int)(newHeight * ratio);
            }

            var resizedImage = new Bitmap(newWidth, newHeight);
            using (var graphics = Graphics.FromImage(resizedImage))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }

            return resizedImage;
        }

        // Cập nhật LoadAvatar method
        private async Task LoadAvatar()
        {
            if (_currentUser?.AvatarUrl != null && !string.IsNullOrEmpty(_currentUser.AvatarUrl))
            {
                try
                {
                    if (_currentUser.AvatarUrl.StartsWith("http"))
                    {
                        // Load từ internet
                        var imageData = await Task.Run(async () =>
                        {
                            using (var client = new System.Net.Http.HttpClient())
                            {
                                client.Timeout = TimeSpan.FromSeconds(10);
                                return await client.GetByteArrayAsync(_currentUser.AvatarUrl);
                            }
                        });

                        // Process image trong background
                        var processedData = await Task.Run(() =>
                        {
                            using (var ms = new MemoryStream(imageData))
                            using (var originalImage = Image.FromStream(ms))
                            using (var resizedImage = ResizeImage(originalImage, 200, 200))
                            {
                                using (var outputMs = new MemoryStream())
                                {
                                    resizedImage.Save(outputMs, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    return outputMs.ToArray();
                                }
                            }
                        });

                        // Update UI
                        UpdateAvatarUI(processedData);
                    }
                    else
                    {
                        // Load local file
                        string avatarPath = Path.IsPathRooted(_currentUser.AvatarUrl)
                            ? _currentUser.AvatarUrl
                            : Path.Combine(Application.StartupPath, _currentUser.AvatarUrl);

                        if (File.Exists(avatarPath))
                        {
                            var processedData = await ProcessImageAsync(avatarPath);
                            UpdateAvatarUI(processedData);
                        }
                        else
                        {
                            SetDefaultAvatar();
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading avatar: {ex.Message}");
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
            if (picAvatar.Image != null)
            {
                var oldImage = picAvatar.Image;
                picAvatar.Image = null;
                oldImage.Dispose();
            }

            var bitmap = new Bitmap(100, 100);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.FillEllipse(Brushes.LightGray, 0, 0, 100, 100);
                string initial = _currentUser?.Username?.Substring(0, 1).ToUpper() ?? "U";
                g.DrawString(initial, new Font("Segoe UI", 24, FontStyle.Bold), Brushes.White, 35, 30);
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

        /// <summary>
        /// Refresh the coins display in the Account control
        /// </summary>
        public async void RefreshCoinsDisplay()
        {
            try
            {
                if (_currentUser != null && _userService != null)
                {
                    var coins = await _userService.GetUserCoinsAsync(_currentUser.UserId);

                    // Update UI on main thread
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() => lblCoins.Text = $"{coins:N0} xu"));
                    }
                    else
                    {
                        lblCoins.Text = $"{coins:N0} xu";
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error refreshing coins display: {ex.Message}");
            }
        }
    }
}