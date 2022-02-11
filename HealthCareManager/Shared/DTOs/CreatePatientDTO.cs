using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCareManager.Shared.DTOs
{
    public record CreatePatientDTO
    {
        public string RfidTagId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string OtherInformation { get; set; } = string.Empty;
    }
}
