namespace Library.ControllersApi.Services;

using Library.Data.Entities;

public interface IUserService
{
    Task<string?> RegisterAsync(string username, string password);

    Task<User?> ValidateAsync(string username, string password);
}
