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

        [AllowAnonymous]
        [HttpGet("seed")]
        public async Task<IActionResult> SeedData()
        {
            var all = _dbContext.Patients.ToList();
            _dbContext.RemoveRange(all);
            await _dbContext.SaveChangesAsync();

            var patient = new Patient
            {
                RfidTagId = "12345",
                FullName = "Subject Under Test",
                Gender = "Test Gender",
                OtherInformation = "This is some " +
                "verrrryyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy long text",
                MedicalHistory = new()
                {
                    new() { Info = "Some Info", Time = DateTime.Today.AddDays(-10) },
                    new() { Info = "Some Other Info", Time = DateTime.Today.AddDays(-5) }                    
                },
                MedicalInformation = new()
                {
                    new(){ Key = "Blood Group", Value = "O+"},
                    new(){ Key = "Genotype", Value = "AA"}
                },
                Prescriptions = new()
                {
                    new()
                    {
                        MedicationName = "Test Medicine 1",
                        DosageDescription = "1 per day",
                        StartDate = DateTime.Today.AddDays(-10),
                        EndDate = DateTime.Today.AddDays(-5),
                    },
                    new()
                    {
                        MedicationName = "Test Medicine 2",
                        DosageDescription = "2 per day",
                        StartDate = DateTime.Today.AddDays(-10),
                        EndDate = DateTime.Today.AddDays(-5),
                    }
                }

            };

            _dbContext.Patients.Add(patient);
            await _dbContext.SaveChangesAsync();

            //if(result.Succeeded)
            return Ok(AppResponse.Success("Data seeding successful"));

            //return BadRequest(AppResponse.Error("Unknown Error Occurred"));
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);

            if (user == null)
                return BadRequest(AppResponse.Error("User deos not exist"));

            if(!await _userManager.CheckPasswordAsync(user, dto.Password))
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
            catch(Exception ex)
            {
                return BadRequest(AppResponse.Error(ex.Message));
            }
        }

        [HttpGet("patient-data/{rfid}")]
        public async Task<IActionResult> GetPatient(string rfid)
        {
            string userTypetring = User.FindFirstValue(nameof(UserType));
            UserType userType = (UserType) Enum.Parse(typeof(UserType), userTypetring);

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
        public async Task<IActionResult> UpdatePatient(Patient patient )
        {
            await _dbContext.UpdatePatient(patient);
            return Ok(AppResponse.Success("Patient Updated Successfully"));
        }
        
        [HttpPut("patient/{patientId}/prescription/{prescriptionId}")]
        public async Task<IActionResult> MarkCollectedPrescription(int patientId, int prescriptionId)
        {
            string userTypetring = User.FindFirstValue(nameof(UserType));
            UserType userType = (UserType)Enum.Parse(typeof(UserType), userTypetring);

            if (userType is not UserType.Admin)
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
