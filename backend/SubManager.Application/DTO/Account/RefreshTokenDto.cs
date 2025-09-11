using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubManager.Application.DTO.Account
{
    public class RefreshTokenDto
    {
        public string Token { get; set; } = string.Empty;
        public bool IsRevoked { get; set; }
        public DateTime Expire { get; set; }
    }
}
