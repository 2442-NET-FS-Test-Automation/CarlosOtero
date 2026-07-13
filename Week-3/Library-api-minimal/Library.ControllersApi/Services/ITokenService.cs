namespace Library.ControllersApi.Services;

public interface ITokenService
{
    string Issue(string user);
}