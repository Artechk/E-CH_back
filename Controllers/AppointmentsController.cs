using E_CH_back.Models;
using E_CH_back.Services;
using Microsoft.AspNetCore.Mvc;

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

            // Проверяем доступное время
            var timeSlot = doctor.AvailableTimes.FirstOrDefault(t => t.StartTime <= request.AppointmentTime && t.EndTime > request.AppointmentTime);
            if (timeSlot == null)
                return BadRequest(new { message = "The selected time is not available" });

            // Убираем слот из доступных
            doctor.AvailableTimes.Remove(timeSlot);
            await _doctorService.UpdateDoctorAsync(doctor);

            // Создаём запись
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

    }
}
