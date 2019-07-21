﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VollyV2.Models;

namespace VollyV2.Data.Volly
{
    public class Application
    {
        public int Id { get; set; }
        public virtual Opportunity Opportunity { get; set; }
        [JsonIgnore]
        public virtual List<ApplicationOccurrence> Occurrences { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
