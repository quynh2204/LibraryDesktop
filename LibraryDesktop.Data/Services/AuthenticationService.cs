using System.Security.Cryptography;
using System.Text;
using LibraryDesktop.Data.Interfaces;
using LibraryDesktop.Models;

namespace LibraryDesktop.Data.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserSettingRepository _userSettingRepository;

        public AuthenticationService(IUserRepository userRepository, IUserSettingRepository userSettingRepository)
        {
            _userRepository = userRepository;
            _userSettingRepository = userSettingRepository;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user != null && VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }

        public async Task<User?> RegisterAsync(string username, string email, string password)
        {
            // Check if username or email already exists
            if (await _userRepository.UsernameExistsAsync(username))
                return null;
            
            if (await _userRepository.EmailExistsAsync(email))
                return null;

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = HashPassword(password),
                RegistrationDate = DateTime.Now
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Create default user settings
            var userSetting = new UserSetting
            {
                UserId = user.UserId,
                ThemeMode = ThemeMode.Light,
                FontSize = 12,
                Balance = 0
            };

            await _userSettingRepository.AddAsync(userSetting);
            await _userSettingRepository.SaveChangesAsync();

            return user;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null && VerifyPassword(currentPassword, user.PasswordHash))
            {
                user.PasswordHash = HashPassword(newPassword);
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = password + "LibraryApp_Salt_2024";
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public bool VerifyPassword(string password, string hash)
        {
            var hashedPassword = HashPassword(password);
            return hashedPassword == hash;
        }
    }
}
