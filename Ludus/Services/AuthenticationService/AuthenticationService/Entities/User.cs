using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Entities
{
        public class User : IdentityUser
        {
            public string mlb { get; set; }
            public string name { get; set; }
            public string surname { get; set; }




        }
    
}
