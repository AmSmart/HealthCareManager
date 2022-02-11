using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCareManager.Shared.Models;
public class Patient
{
    public int Id { get; set; }
    
    public string RfidTagId { get; set; } = string.Empty;
    
    public string FullName { get; set; } = string.Empty;
    
    public string Gender { get; set; } = string.Empty;
    
    public string OtherInformation { get; set; } = string.Empty;

    [Column(TypeName = "jsonb")]
    public List<Prescription> Prescriptions { get; set; } = new();
    
    [Column(TypeName = "jsonb")]
    public Dictionary<string, string> MedicalInformation { get; set; } = new();
    
    [Column(TypeName = "jsonb")]
    public List<MedicalEvent> MedicalHistory { get; set; } = new();
}

public class Prescription
{
    public int Id { get; set; }

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
