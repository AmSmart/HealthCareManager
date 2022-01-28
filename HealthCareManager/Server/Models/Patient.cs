using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCareManager.Server.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string RfidTagId { get; set; } = string.Empty;
        public List<Prescription> Prescriptions { get; set; } = new();
        public Dictionary<string, string> MedicalInformation { get; set; } = new();
        public List<MedicalEvent> MedicalHistory { get; set; } = new();
    }

    public class Prescription
    {
        public string MedicationName { get; set; } = string.Empty;

        public string DosageDescription { get; set; } = string.Empty;

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public int AllowedCount { get; set; }

        public int CollectedCount { get; set; }
    }

    public class MedicalEvent
    {
        public DateTime Time { get; set; }

        public string Info { get; set; } = string.Empty;
    }
}
