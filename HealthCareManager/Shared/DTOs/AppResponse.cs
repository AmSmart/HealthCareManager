using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCareManager.Shared.DTOs
{
    public class AppResponse<T>
    {
        public AppResponse() { }
        
        public AppResponse(T data, string message = "Success") 
        {
            Succeded = true;
            Message = message;
            Data = data;
        }

        public bool Succeded { get; set; }
        
        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }

    }

    public class AppResponse : AppResponse<object>
    {
        public static AppResponse Success(string message) 
            => new AppResponse 
            {
                Message = message,
                Succeded = true,
                Data = null
            };

        public static AppResponse Error(string message)
             => new AppResponse
             {
                 Message = message,
                 Succeded = false,
                 Data = null
             };
    }
}
