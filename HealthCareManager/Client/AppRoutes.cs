using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCareManager.Client
{
    public static class AppRoutes
    {
        public const string Index = "/";
        
        public const string Login = "/login";
     
        public const string AdminDashboard = "/dashboard/admin";
        
        public const string DoctorDashboard = "/dashboard/doctor";
        
        public const string PharmacistDashboard = "/dashboard/pharmacist";
    }
}
