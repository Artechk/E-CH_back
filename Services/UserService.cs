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

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _context.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();
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

        public async Task UpdateUserAsync(User user)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            await _context.Users.ReplaceOneAsync(filter, user);
        }

        public async Task AddAddressAsync(string userId, Address address)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            user.Addresses.Add(address);
            await UpdateUserAsync(user);
        }

        public async Task UpdateAddressAsync(string userId, string addressId, Address updatedAddress)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            var address = user.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null) throw new Exception("Address not found");

            address.Street = updatedAddress.Street;
            address.City = updatedAddress.City;
            address.State = updatedAddress.State;
            address.ZipCode = updatedAddress.ZipCode;

            await UpdateUserAsync(user);
        }

        public async Task DeleteAddressAsync(string userId, string addressId)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            var address = user.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null) throw new Exception("Address not found");

            user.Addresses.Remove(address);
            await UpdateUserAsync(user);
        }

    }
}
