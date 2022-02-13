using HealthCareManager.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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

        public async Task<Patient> CreatePatientAsync(Patient patient)
        {
            patient.Id = 0;
            await Patients.AddAsync(patient);
            await SaveChangesAsync();
            return patient;
        }
        
        public async Task UpdatePatient(Patient patient)
        {
            Patients.Update(patient);
            await SaveChangesAsync();
        }

        public async Task<Patient> GetPatientByIdAsync(int id) 
            => await Patients.FirstAsync(x => x.Id == id);
        
        public async Task<Patient> GetPatientByRfidAsync(string rfid) 
            => await Patients.FirstAsync(x => x.RfidTagId == rfid);
    }
}