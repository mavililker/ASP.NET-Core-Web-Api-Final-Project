using Microsoft.AspNetCore.Identity;
using System;

namespace EventProjectFinal.Models
{
    public class AppUserTokens : IdentityUserToken<string>
    {
        public DateTime ExpireDate { get; set; }
    }
}
