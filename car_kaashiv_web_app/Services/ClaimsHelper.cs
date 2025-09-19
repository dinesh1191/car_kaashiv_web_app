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
        public static List<Claim> BuildUserClaimsEmp(TableEmployee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            return new List<Claim>
            {
                new Claim(ClaimTypes.Name, employee?.Name ?? string.Empty),
                new Claim(ClaimTypes.MobilePhone, employee?.Phone ?? string.Empty),
                new Claim(ClaimTypes.Role, employee?.Role ?? string.Empty)
            };
        }
    }

}
