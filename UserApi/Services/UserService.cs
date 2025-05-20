using System.Security.Claims;
using UserApi.Models.DTO;
using System;

namespace UserApi.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        private string CurrentUserLogin =>
            _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        private bool IsAdmin =>
            _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;

        public async Task<User> CreateUserAsync(UserCreateDto dto)
        {
            if (!IsAdmin)
                throw new UnauthorizedAccessException("Only admins can create users");

            if (!await _userRepository.IsLoginUniqueAsync(dto.Login))
                throw new ArgumentException("Login must be unique");

            var newUser = new User
            {
                Login = dto.Login,
                Password = dto.Password,
                Name = dto.Name,
                Gender = dto.Gender,
                Birthday = dto.Birthday,
                Admin = dto.IsAdmin,
                CreatedBy = CurrentUserLogin
            };

            return await _userRepository.AddAsync(newUser);
        }

        public async Task UpdateUserInfoAsync(string login, UserUpdateDto dto)
        {
            var user = await GetUserForUpdateAsync(login);

            user.Name = dto.Name ?? user.Name;
            user.Gender = dto.Gender ?? user.Gender;
            user.Birthday = dto.Birthday ?? user.Birthday;

            user.ModifiedOn = DateTime.UtcNow;
            user.ModifiedBy = CurrentUserLogin;

            await _userRepository.UpdateUserAsync(user);
        }

        public async Task ChangePasswordAsync(string login, string newPassword)
        {
            var user = await GetUserForUpdateAsync(login);

            if (!IsAdmin && user.Password != CurrentUserLogin)
                throw new UnauthorizedAccessException("Invalid credentials");

            user.Password = newPassword;
            user.ModifiedOn = DateTime.UtcNow;
            user.ModifiedBy = CurrentUserLogin;

            await _userRepository.UpdateUserAsync(user);
        }

        public async Task ChangeLoginAsync(string oldLogin, string newLogin)
        {
            var user = await GetUserForUpdateAsync(oldLogin);

            if (!await _userRepository.IsLoginUniqueAsync(newLogin))
                throw new ArgumentException("New login is already taken");

            user.Login = newLogin;
            user.ModifiedOn = DateTime.UtcNow;
            user.ModifiedBy = CurrentUserLogin;

            await _userRepository.UpdateUserAsync(user);
        }

        public async Task<List<User>> GetAllActiveUsersAsync()
        {
            if (!IsAdmin)
                throw new UnauthorizedAccessException("Admin access required");

            return await _userRepository.GetAllActiveAsync();
        }

        public async Task<User> GetUserByLoginAsync(string login)
        {
            return await _userRepository.GetByLoginAsync(login);
        }

        public async Task<User> GetPersonalInfoAsync(string password)
        {
            var user = await _userRepository.GetByLoginAsync(CurrentUserLogin);

            if (user == null || user.Password != password || user.RevokedOn != null)
                throw new UnauthorizedAccessException("Invalid credentials");

            return user;
        }

        public async Task<List<User>> GetUsersOlderThanAsync(int age)
        {
            if (!IsAdmin)
                throw new UnauthorizedAccessException("Admin access required");

            return await _userRepository.GetUsersOlderThanAgeAsync(age);
        }

        public async Task SoftDeleteAsync(string login)
        {
            if (!IsAdmin)
                throw new UnauthorizedAccessException("Admin access required");

            await _userRepository.SoftDeleteAsync(login, CurrentUserLogin);
        }
        public async Task DeleteAsync(string login)
        {
            if (!IsAdmin)
                throw new UnauthorizedAccessException("Admin access required");

            await _userRepository.DeleteAsync(login);
        }

        public async Task RestoreUserAsync(string login)
        {
            if (!IsAdmin)
                throw new UnauthorizedAccessException("Admin access required");

            await _userRepository.RestoreUserAsync(login);
        }

        private async Task<User> GetUserForUpdateAsync(string login)
        {
            var user = await _userRepository.GetByLoginAsync(login)
                ?? throw new KeyNotFoundException("User not found");

            if (user.RevokedOn != null)
                throw new InvalidOperationException("User is deleted");

            if (!IsAdmin && user.Login != CurrentUserLogin)
                throw new UnauthorizedAccessException("Access denied");

            return user;
        }
    }
}