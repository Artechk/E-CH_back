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

            // Проверяем, есть ли свободный слот на указанную дату и время
            var appointmentTime = request.AppointmentTime;
            var timeSlot = doctor.AppointmentTimes.FirstOrDefault(t => t == appointmentTime); // Сравниваем точную дату и время
            if (timeSlot == null)
                return BadRequest(new { message = "The selected time is not available" });

            // Убираем слот из доступных
            doctor.AppointmentTimes.Remove(timeSlot);
            await _doctorService.UpdateDoctorAsync(doctor);

            // Создаём запись с полной датой и временем
            var appointment = new Appointment
            {
                UserId = request.UserId,
                DoctorId = request.DoctorId,
                AppointmentTime = request.AppointmentTime, // Полный DateTime (с датой и временем)
                Workplace = doctor.Workplace
            };
            await _appointmentService.AddAppointmentAsync(appointment);

            return Ok(new { message = "Appointment successfully booked" });
        }
    }
}
