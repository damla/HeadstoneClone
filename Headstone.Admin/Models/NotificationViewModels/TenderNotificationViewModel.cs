﻿using Headstone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.AI.Models.ViewModels
{
    public class TenderNotificationViewModel
    {
        public UserViewModel User { get; set; }

        public string SiteRoot { get; set; }
    }
}