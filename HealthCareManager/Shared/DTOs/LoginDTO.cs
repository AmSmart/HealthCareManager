using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCareManager.Shared.DTOs
{
    public record LoginDTO
    {
        public string UserName { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
