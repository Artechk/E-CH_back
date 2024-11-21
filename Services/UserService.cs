using BCrypt.Net;
using MongoDB.Driver;
using E_CH_back.Models;
using E_CH_back.Data;

namespace E_CH_back.Services
{
    public class UserService
    {
        private readonly MongoDbContext _context;

        public UserService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<string> AddUser(User user)
        {
            // Используем BCrypt.Net для хэширования пароля
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            await _context.Users.InsertOneAsync(user);
            return user.Id;
        }

        public async Task<bool> UserExists(string email)
        {
            var user = await _context.Users.Find(u => u.Email == email).FirstOrDefaultAsync();
            return user != null;
        }

        public async Task<bool> AuthenticateUser(string email, string password)
        {
            var user = await _context.Users.Find(u => u.Email == email).FirstOrDefaultAsync();
            if (user == null) return false;

            // Используем BCrypt.Net для проверки пароля
            return BCrypt.Net.BCrypt.Verify(password, user.Password);
        }
    }
}
