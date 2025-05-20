using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = new();
    private readonly object _lock = new();

    public Task<User> AddAsync(User user)
    {
        lock (_lock)
        {
            if (_users.Any(u => u.Login == user.Login))
                throw new InvalidOperationException("Login already exists");

            _users.Add(user);
            return Task.FromResult(user);
        }
    }

    public Task<User> GetByLoginAsync(string login)
    {
        return Task.FromResult(_users.FirstOrDefault(u =>
            u.Login.Equals(login, StringComparison.OrdinalIgnoreCase)));
    }

    public Task<List<User>> GetAllActiveAsync()
    {
        return Task.FromResult(_users
            .Where(u => u.RevokedOn == null)
            .OrderBy(u => u.CreatedOn)
            .ToList());
    }

    public Task UpdateUserAsync(User user)
    {
        lock (_lock)
        {
            var index = _users.FindIndex(u => u.Id == user.Id);
            if (index >= 0) _users[index] = user;
        }
        return Task.CompletedTask;
    }

    public Task<List<User>> GetUsersOlderThanAgeAsync(int age)
    {
        var now = DateTime.UtcNow;
        return Task.FromResult(_users
            .Where(u => u.Birthday.HasValue &&
                (now.Year - u.Birthday.Value.Year) > age)
            .ToList());
    }

    public Task SoftDeleteAsync(string login, string revokedBy)
    {
        lock (_lock)
        {
            var user = _users.FirstOrDefault(u => u.Login == login);
            if (user != null)
            {
                user.RevokedOn = DateTime.UtcNow;
                user.RevokedBy = revokedBy;
            }
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string login)
    {
        lock (_lock)
        {
            var user = _users.FirstOrDefault(u => u.Login == login);
            if (user != null) _users.Remove(user);
        }
        return Task.CompletedTask;
    }

    public Task RestoreUserAsync(string login)
    {
        lock (_lock)
        {
            var user = _users.FirstOrDefault(u => u.Login == login);
            if (user != null)
            {
                user.RevokedOn = null;
                user.RevokedBy = null;
            }
        }
        return Task.CompletedTask;
    }

    public Task<bool> IsLoginUniqueAsync(string login)
    {
        return Task.FromResult(!_users.Any(u =>
            u.Login.Equals(login, StringComparison.OrdinalIgnoreCase)));
    }

    public Task<List<User>> GetAllUsersAsync()
    {
        return Task.FromResult(_users.ToList());
    }
}