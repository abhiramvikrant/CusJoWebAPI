using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CusJoWebAPI.Models
{
 
        public enum Gender
        {
            Male, Female
        }

        public class ApplicationUser : IdentityUser
        {
            public string FirstName { get; set; }
            public string LastNamre { get; set; }
            public string MiddleName { get; set; }

            public Gender Gender { get; set; }

            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string City { get; set; }

            public string ZipCode { get; set; }

            public string State { get; set; }

            public string Country { get; set; }

        }
    
}
