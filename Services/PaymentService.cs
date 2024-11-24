using E_CH_back.Data;
using E_CH_back.Models;
using MongoDB.Driver;

namespace E_CH_back.Services
{
    public class PaymentService
    {
        private readonly IMongoCollection<Payment> _payments;

        public PaymentService(MongoDbContext context)
        {
            _payments = context.Payments;
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            await _payments.InsertOneAsync(payment);
        }

        public async Task<List<Payment>> GetPaymentsByUserIdAsync(string userId)
        {
            return await _payments.Find(p => p.UserId == userId).ToListAsync();
        }

        public async Task<List<Payment>> GetPaymentsByAddressIdAsync(string addressId)
        {
            return await _payments.Find(p => p.AddressId == addressId).ToListAsync();
        }

        public async Task<List<Payment>> GetPaymentsByPeriodAsync(string userId, DateTime startDate, DateTime endDate)
        {
            var filter = Builders<Payment>.Filter.And(
                Builders<Payment>.Filter.Eq(p => p.UserId, userId),
                Builders<Payment>.Filter.Gte(p => p.PaymentDate, startDate),
                Builders<Payment>.Filter.Lte(p => p.PaymentDate, endDate)
            );
            return await _payments.Find(filter).ToListAsync();
        }

        public async Task<List<Payment>> GetPaymentsByTypeAsync(string userId, PaymentType type)
        {
            var filter = Builders<Payment>.Filter.And(
                Builders<Payment>.Filter.Eq(p => p.UserId, userId),
                Builders<Payment>.Filter.Eq(p => p.PaymentType, type)
            );
            return await _payments.Find(filter).ToListAsync();
        }
    }
}
