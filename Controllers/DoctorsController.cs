using E_CH_back.Models;
using E_CH_back.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_CH_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly DoctorService _doctorService;

        public DoctorsController(DoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpPost]
        public async Task<IActionResult> AddDoctor([FromBody] Doctor doctor)
        {
            await _doctorService.AddDoctor(doctor);
            return Ok(new { message = "Doctor added successfully", doctorId = doctor.Id });
        }
    }
}
