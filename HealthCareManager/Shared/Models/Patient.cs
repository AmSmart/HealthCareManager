using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public List<MedicalInfo> MedicalInformation { get; set; } = new();
    
    [Column(TypeName = "jsonb")]
    public List<MedicalEvent> MedicalHistory { get; set; } = new();
}

public class Prescription
{
    public int Id { get; set; }

    [Required]
    public string MedicationName { get; set; } = string.Empty;

    [Required]
    public string DosageDescription { get; set; } = string.Empty;

    [Required]
    public DateTime? StartDate { get; set; }

    [Required]
    public DateTime? EndDate { get; set; }

    [Required]
    public bool Collected { get; set;  }
}

public class MedicalEvent
{
    public DateTime? Time { get; set; }

    [Required]
    public string Info { get; set; } = string.Empty;
}

public class MedicalInfo
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
