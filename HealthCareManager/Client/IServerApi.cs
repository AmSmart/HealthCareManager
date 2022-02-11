using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCareManager.Shared.DTOs;
using HealthCareManager.Shared.Models;
using Refit;

namespace HealthCareManager.Client
{
    public interface IServerApi
    {
        [Post("/api/login")]
        Task<AppResponse<JwtToken>> LoginAsync([Body] LoginDTO dto);
        
        [Post("/api/user")]
        Task<AppResponse> RegisterUserAsync(CreateUserDTO dto);

        [Get("/api/patient-data/{rfid}")]
        Task<AppResponse<Patient>> GetPatientAsync(string rfid);

        [Post("/api/patient")]
        Task<AppResponse> AddPatientAsync([Body] CreatePatientDTO request);

        [Put("/patient/{patientId}/prescription/{prescriptionId}")]
        Task<AppResponse<string>> IncrementCollectedPrescriptionCountAsync(int patientId, int prescriptionId);
    }
}
