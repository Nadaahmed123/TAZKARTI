using System;
using Microsoft.AspNetCore.Identity;

namespace Tazkarti.Models
{
    public class ApplicationRole : IdentityRole
    {
        public DateTime CreateAt { get; set; } = DateTime.Now;
    }
}
