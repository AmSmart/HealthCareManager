using HealthCareManager.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HealthCareManager.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        
        public ApplicationDbContext(
            DbContextOptions options) : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public async Task<Patient> CreatePatient(Patient patient)
        {
            patient.Id = 0;
            await Patients.AddAsync(patient);
            await SaveChangesAsync();
            return patient;
        }
        
        public async Task UpdatePatient(Patient patient)
        {
            patient.Id = 0;
            Patients.Update(patient);
            await SaveChangesAsync();
        }

        public async Task<Patient> GetPatientByIdAsync(int id) 
            => await Patients.FirstAsync(x => x.Id == id);
        
        public async Task<Patient> GetPatientByRfidAsync(string rfid) 
            => await Patients.FirstAsync(x => x.RfidTagId == rfid);

        //public async Task<ApplicationUser> CreateUserAsync(CreateUserDTO dto)
        //{
        //    if (dto.Password != dto.ConfirmPassword)
        //        throw new Exception("Password and Confirm Passwprd do not match");

        //    var userType = Enum.Parse(typeof(UserType), dto.UserType);
        //    if (userType is null)
        //        throw new Exception("Invalid User Type");

        //    if ((UserType)userType == UserType.Admin)
        //        throw new Exception("Cannot create admin user");

        //    var user = new ApplicationUser
        //    {
        //        UserName = dto.UserName,
        //        FullName = dto.FullName,
        //        UserType = (UserType) userType
        //    };

        //    var result = await _userManager.CreateAsync(user, dto.Password);

        //    if(!result.Succeeded)
        //        throw new Exception(string.Join(',', $"{result.Errors.Select(x => x.Description)}"));

        //    return user;
        //}        

    }
}