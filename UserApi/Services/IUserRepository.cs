using System.Collections.Generic;
using System.Threading.Tasks;

public interface IUserRepository
{
	Task DeleteAsync(string login);
	Task<User> AddAsync(User user);
	Task<User> GetByLoginAsync(string login);
	Task<List<User>> GetAllActiveAsync();
	Task UpdateUserAsync(User user);
	Task<List<User>> GetUsersOlderThanAgeAsync(int age);
	Task SoftDeleteAsync(string login, string revokedBy);
	Task RestoreUserAsync(string login);
	Task<bool> IsLoginUniqueAsync(string login);
	Task<List<User>> GetAllUsersAsync();
}