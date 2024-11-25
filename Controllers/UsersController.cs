﻿using Microsoft.AspNetCore.Mvc;
using E_CH_back.Models;
using E_CH_back.Services;
using MongoDB.Bson;

namespace E_CH_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly PaymentService _paymentService;

        public UsersController(UserService userService, PaymentService paymentService)
        {
            _userService = userService;
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            if (await _userService.UserExists(user.Email))
            {
                return Conflict(new { message = "User already exists" });
            }

            var userId = await _userService.AddUser(user);
            return CreatedAtAction(nameof(AddUser), new { id = userId }, new { userId });
        }

        [HttpGet("check")]
        public async Task<IActionResult> CheckUserExists([FromQuery] string email)
        {
            var exists = await _userService.UserExists(email);
            return Ok(new { exists });
        }


        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User user)
        {
            var authenticatedUser = await _userService.AuthenticateUser(user.Email, user.Password);
            if (authenticatedUser == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            return Ok(new { message = "Authenticated successfully" });
        }




        [HttpPost("{userId}/addresses")]
        public async Task<IActionResult> AddAddress(string userId, [FromBody] Address address)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            address.Id = ObjectId.GenerateNewId().ToString(); // Генерация ID для адреса
            user.Addresses.Add(address);
            await _userService.UpdateUserAsync(user);

            return Ok(new { message = "Address added successfully", addressId = address.Id });
        }

        [HttpGet("{userId}/addresses")]
        public async Task<IActionResult> GetAddresses(string userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user.Addresses);
        }

        [HttpPut("{userId}/addresses/{addressId}")]
        public async Task<IActionResult> UpdateAddress(string userId, string addressId, [FromBody] Address updatedAddress)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            var address = user.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
                return NotFound(new { message = "Address not found" });

            address.Street = updatedAddress.Street;
            address.City = updatedAddress.City;
            address.State = updatedAddress.State;
            address.ZipCode = updatedAddress.ZipCode;

            await _userService.UpdateUserAsync(user);

            return Ok(new { message = "Address updated successfully" });
        }

        [HttpDelete("{userId}/addresses/{addressId}")]
        public async Task<IActionResult> DeleteAddress(string userId, string addressId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            var address = user.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
                return NotFound(new { message = "Address not found" });

            user.Addresses.Remove(address);
            await _userService.UpdateUserAsync(user);

            return Ok(new { message = "Address deleted successfully" });
        }

        [HttpPost("{userId}/addresses/{addressId}/payments")]
        public async Task<IActionResult> MakePayment(string userId, string addressId, [FromBody] Payment payment)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            var address = user.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
                return NotFound(new { message = "Address not found" });

            payment.UserId = userId;
            payment.AddressId = addressId;
            payment.PaymentDate = DateTime.UtcNow; 

            await _paymentService.AddPaymentAsync(payment);

            return Ok(new { message = "Payment successfully processed", paymentId = payment.Id });
        }


        [HttpGet("{userId}/payments")]
        public async Task<IActionResult> GetUserPayments(string userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            var payments = await _paymentService.GetPaymentsByUserIdAsync(userId);

            return Ok(payments);
        }

        [HttpGet("{userId}/addresses/{addressId}/payments")]
        public async Task<IActionResult> GetAddressPayments(string userId, string addressId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            var address = user.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
                return NotFound(new { message = "Address not found" });

            var payments = await _paymentService.GetPaymentsByAddressIdAsync(addressId);

            return Ok(payments);
        }

    }
}
