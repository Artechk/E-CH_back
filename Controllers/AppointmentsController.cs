using E_CH_back.Models;
using E_CH_back.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace E_CH_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppointmentService _appointmentService;
        private readonly DoctorService _doctorService;

        public AppointmentsController(AppointmentService appointmentService, DoctorService doctorService)
        {
            _appointmentService = appointmentService;
            _doctorService = doctorService;
        }

        [HttpPost("book")]
        public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentRequest request)
        {
            // Проверяем врача
            var doctor = await _doctorService.GetDoctorByIdAsync(request.DoctorId);
            if (doctor == null)
                return NotFound(new { message = "Doctor not found" });

            // Проверяем, есть ли свободный слот на указанную дату и время
            var appointmentTime = request.AppointmentTime;

            if (!doctor.AppointmentTimes.Contains(appointmentTime))
            {
                return BadRequest(new { message = "The selected time is not available" });
            }

            // Проверяем, есть ли уже запись на это время у этого врача
            var existingAppointment = await _appointmentService.GetAppointmentByDoctorAndTimeAsync(request.DoctorId, appointmentTime);
            if (existingAppointment != null)
            {
                return BadRequest(new { message = "The selected time is already booked" });
            }

            // Убираем слот из доступных
            doctor.AppointmentTimes.Remove(appointmentTime);
            await _doctorService.UpdateDoctorAsync(doctor);

            // Создаём запись с полной датой и временем
            var appointment = new Appointment
            {
                UserId = request.UserId,
                DoctorId = request.DoctorId,
                AppointmentTime = request.AppointmentTime,
                Workplace = doctor.Workplace
            };
            await _appointmentService.AddAppointmentAsync(appointment);

            return Ok(new { message = "Appointment successfully booked" });
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAppointmentsByUserId(string userId)
        {
            var appointments = await _appointmentService.GetAppointmentsByUserIdAsync(userId);
            return Ok(appointments);
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetAppointmentsByDoctorId(string doctorId)
        {
            var appointments = await _appointmentService.GetAppointmentsByDoctorIdAsync(doctorId);
            return Ok(appointments);
        }

        [HttpDelete("{appointmentId}")]
        public async Task<IActionResult> DeleteAppointment(string appointmentId)
        {
            // Ищем запись по ID
            var appointment = await _appointmentService.GetAppointmentsByDoctorIdAsync(appointmentId);

            if (appointment == null)
            {
                return NotFound(new { message = "Appointment not found" });
            }

            // Удаляем запись
            var filter = Builders<Appointment>.Filter.Eq(a => a.Id, appointmentId);
            var result = await _appointmentService.DeleteAppointmentAsync(filter);

            if (result.DeletedCount > 0)
            {
                return Ok(new { message = "Appointment successfully deleted" });
            }
            else
            {
                return BadRequest(new { message = "Error deleting appointment" });
            }
        }

    }
}
