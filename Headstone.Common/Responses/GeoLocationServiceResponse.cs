﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Common.Responses
{
    public class GeoLocationServiceResponse<T> : ServiceResponse<T>
    {
        public int ApplicationId { get; set; }
    }
}
