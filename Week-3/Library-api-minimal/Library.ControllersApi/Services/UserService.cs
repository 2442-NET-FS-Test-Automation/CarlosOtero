namespace Library.ControllersApi.Services;

using Library.Data;
using Library.Data.Entities;

using Microsoft.AspNetCore.Identity; // Not the full framework - we just need the PasswordHasher
using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{
    private readonly LibraryDbContext _db;

    // comes from ASP.NET Identity. Uses per-password salt to obfuscate/hash passwords
    // we will hash THEN store. And always verify against that hash. Never store plaintext passwords.
    // Generally: don't invent your own hashing
    private readonly IPasswordHasher<User> _hasher;

    public UserService(LibraryDbContext db, IPasswordHasher<User> hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    public async Task<string?> RegisterAsync(string username, string password){
        // First - trim the string
        string name = username.Trim();

        // Check to see if username is already taken
        if (await _db.Users.AnyAsync(u=>u.UserName == name))
        {
            return "username is taken";
        }

        User newUser = new User{UserName = name, Role = "consumer"}; // Never trust the client on the role
        
        // Hashing + salting password
        newUser.PasswordHash = _hasher.HashPassword(newUser,password);

        _db.Users.Add(newUser);
        await _db.SaveChangesAsync();
        return null; // if all goes well, return null
    }

    public async Task<User?> ValidateAsync(string username, string password){
        User? foundUser = await _db.Users.SingleOrDefaultAsync(u=> u.UserName == username);

        if(foundUser is null) return null; // Unknown username and wrong pass look IDENTICAL
        // probably not the best implementation - you guys can do more checks later.

        // using the hasher to verify a hashed password
        var result = _hasher.VerifyHashedPassword(foundUser, foundUser.PasswordHash, password);

        return result == PasswordVerificationResult.Failed ? null : foundUser;
    }
}