using HealthCareManager.Server.Data;
using HealthCareManager.Shared.DTOs;
using HealthCareManager.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HealthCareManager.Server.Contollers
{
    [Authorize]
    [ApiController]
    [Route("api")]
    public class AppController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppController(ApplicationDbContext dbContext, UserService userService,
            UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userService = userService;
            _userManager = userManager;
        }

        //[AllowAnonymous]
        //[HttpGet("seed")]
        //public async Task<IActionResult> SeedData()
        //{

        //    var admin = await _userManager.FindByNameAsync("admin");
        //    var doctor = await _userManager.FindByNameAsync("doctor");
        //    var pharmacist = await _userManager.FindByNameAsync("pharmacist");

        //    await _userManager.AddClaimAsync(admin, new Claim("UserType", "Admin"));
        //    await _userManager.AddClaimAsync(doctor, new Claim("UserType", "Doctor"));
        //    await _userManager.AddClaimAsync(pharmacist, new Claim("UserType", "Pharmacist"));

        //    return Ok(AppResponse.Success("Data seeding successful"));
        //}

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);

            if (user == null)
                return BadRequest(AppResponse.Error("User deos not exist"));

            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return BadRequest(AppResponse.Error("Invalid Login Credentials"));
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var jwt = _userService.GenerateTokensAsync(claims.ToArray(), DateTime.UtcNow);

            return Ok(new AppResponse<JwtToken>(jwt));
        }

        [HttpPost("user")]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserDTO dto)
        {
            try
            {
                var user = await _userService.CreateUserAsync(dto);
                return Ok(AppResponse.Success($"User '{user.UserName}' Created Successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(AppResponse.Error(ex.Message));
            }
        }

        [HttpGet("patient-data/{rfid}")]
        public async Task<IActionResult> GetPatient(string rfid)
        {
            string userTypetring = User.FindFirstValue(nameof(UserType));
            UserType userType = (UserType)Enum.Parse(typeof(UserType), userTypetring);

            if (userType is UserType.Admin)
                return BadRequest(AppResponse.Error("User Health Data is redacted"));

            var patient = await _dbContext.GetPatientByRfidAsync(rfid);

            if (patient is null)
                return BadRequest(AppResponse.Error("Patient Not Found"));

            switch (userType)
            {
                case UserType.Doctor:
                    return Ok(new AppResponse<Patient>(patient));

                case UserType.Pharmacist:
                    return Ok(new AppResponse<Patient>(patient));

                default:
                    throw new Exception("Unexpected User Type");
            }
        }

        [HttpPost("patient")]
        public async Task<IActionResult> AddPatient([FromBody] CreatePatientDTO dto)
        {
            string userTypetring = User.FindFirstValue(nameof(UserType));
            UserType userType = (UserType)Enum.Parse(typeof(UserType), userTypetring);

            if (userType is not UserType.Admin)
                return Unauthorized();

            await _dbContext.CreatePatientAsync(new Patient
            {
                FullName = dto.FullName,
                Gender = dto.Gender,
                OtherInformation = dto.OtherInformation,
                RfidTagId = dto.RfidTagId
            });

            return Ok(AppResponse.Success("Patient Created Successfully"));
        }

        [HttpPut("patient")]
        public async Task<IActionResult> UpdatePatient(Patient patient)
        {
            await _dbContext.UpdatePatient(patient);
            return Ok(AppResponse.Success("Patient Updated Successfully"));
        }

        [HttpPut("patient/{patientId}/prescription/{prescriptionId}")]
        public async Task<IActionResult> MarkCollectedPrescription(int patientId, int prescriptionId)
        {
            string userTypetring = User.FindFirstValue(nameof(UserType));
            UserType userType = (UserType)Enum.Parse(typeof(UserType), userTypetring);

            if (userType is UserType.Admin)
                return Unauthorized();

            var patient = await _dbContext.GetPatientByIdAsync(patientId);
            var prescription = patient.Prescriptions.First(x => x.Id == prescriptionId);
            prescription.Collected = true;

            _dbContext.Update(patient);
            await _dbContext.SaveChangesAsync();

            return Ok(AppResponse.Success("Prescription Marked Successfully"));
        }

    }
}
