using car_kaashiv_web_app.Models.Entities;
using Microsoft.CodeAnalysis.Operations;
using System.Security.Claims;

namespace car_kaashiv_web_app.Services
{
    public class ClaimsHelper
    {
        public static List<Claim> BuildUserClaims(TableUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return new List<Claim>
            {
                new Claim(ClaimTypes.Name, user?.Name ?? string.Empty),
                new Claim(ClaimTypes.MobilePhone, user?.Phone ?? string.Empty)   
            };
        }
    }

}
