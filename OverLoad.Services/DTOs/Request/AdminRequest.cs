using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverLoad.Services.DTOs.Request
{
    public class ChangeRoleRequest
    {
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
