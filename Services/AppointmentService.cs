using E_CH_back.Models;
using E_CH_back.Data;
using MongoDB.Driver;

namespace E_CH_back.Services
{
    public class AppointmentService
    {
        private readonly IMongoCollection<Appointment> _appointments;

        public AppointmentService(MongoDbContext context)
        {
            _appointments = context.Appointments;
        }

        public async Task AddAppointmentAsync(Appointment appointment)
        {
            await _appointments.InsertOneAsync(appointment);
        }

        public async Task<List<Appointment>> GetAppointmentsByUserIdAsync(string userId)
        {
            return await _appointments.Find(a => a.UserId == userId).ToListAsync();
        }

        public async Task<List<Appointment>> GetAppointmentsByDoctorIdAsync(string doctorId)
        {
            return await _appointments.Find(a => a.DoctorId == doctorId).ToListAsync();
        }
    }
}
