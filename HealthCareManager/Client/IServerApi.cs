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
        Task<ApiResponse<AppResponse<JwtToken>>> LoginAsync([Body] LoginDTO dto);
        
        [Post("/api/user")]
        Task<ApiResponse<AppResponse>> RegisterUserAsync();

        [Get("/api/patient/{rfid}")]
        Task<ApiResponse<AppResponse<Patient>>> GetPatientAsync(string rfid);

        [Post("/api/patient")]
        Task<ApiResponse<AppResponse>> AddPatientAsync([Body] CreatePatientDTO request);

        [Put("/patient/{patientId}/prescription/{prescriptionId}")]
        Task<ApiResponse<AppResponse<string>>> IncrementCollectedPrescriptionCountAsync(int patientId, int prescriptionId);
    }
}
