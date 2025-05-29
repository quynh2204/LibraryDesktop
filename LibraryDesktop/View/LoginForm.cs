using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibraryDesktop.Data.Services;
using LibraryDesktop.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryDesktop.View
{
    public partial class LoginForm : Form
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IServiceProvider _serviceProvider;
        
        public User? AuthenticatedUser { get; private set; }

        public LoginForm(IAuthenticationService authenticationService, IServiceProvider serviceProvider)
        {
            _authenticationService = authenticationService;
            _serviceProvider = serviceProvider;
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                btnLogin.Enabled = false;
                btnLogin.Text = "Logging in...";

                var user = await _authenticationService.LoginAsync(txtUsername.Text, txtPassword.Text);
                
                if (user != null)
                {
                    AuthenticatedUser = user;
                    
                    MessageBox.Show($"Welcome, {user.Username}!", "Login Successful", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.", "Login Failed", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "Login";
            }
        }
        private void btnRegister_Click(object sender, EventArgs e)
        {
            using (var registrationForm = _serviceProvider.GetRequiredService<RegistrationForm>())
            {
                var result = registrationForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    MessageBox.Show("Registration successful! You can now login with your new account.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.Retry;
                    this.Close();
                }
  
            }
        }
    }
}