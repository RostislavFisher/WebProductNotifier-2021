using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProductNotifier.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int ApplicationUserId { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}