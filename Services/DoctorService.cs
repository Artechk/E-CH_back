using E_CH_back.Data;
using E_CH_back.Models;
using MongoDB.Driver;

namespace E_CH_back.Services
{
    public class DoctorService
    {
        private readonly IMongoCollection<Doctor> _doctors;

        public DoctorService(MongoDbContext context)
        {
            _doctors = context.Doctors;
        }

        public async Task AddDoctor(Doctor doctor)
        {
            await _doctors.InsertOneAsync(doctor);
        }

        public async Task<Doctor> GetDoctorByIdAsync(string doctorId)
        {
            return await _doctors.Find(d => d.Id == doctorId).FirstOrDefaultAsync();
        }

        public async Task UpdateDoctorAsync(Doctor doctor)
        {
            var filter = Builders<Doctor>.Filter.Eq(d => d.Id, doctor.Id);
            await _doctors.ReplaceOneAsync(filter, doctor);
        }

        public async Task<List<Doctor>> GetAllDoctorsAsync()
        {
            return await _doctors.Find(_ => true).ToListAsync();
        }

    }
}
