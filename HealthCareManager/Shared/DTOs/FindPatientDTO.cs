﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCareManager.Shared.DTOs;
public record FindPatientDTO
{
    public string RfidTagId { get; set; } = string.Empty;
}
