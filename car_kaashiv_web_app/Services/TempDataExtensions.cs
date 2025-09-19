using Microsoft.AspNetCore.Mvc.ViewFeatures;
using car_kaashiv_web_app.Models.Enums;
using Microsoft.EntityFrameworkCore.Storage;
namespace car_kaashiv_web_app.Services.Extensions
{
    public static class TempDataExtensions
    {
        private const string AlertMessageKey = "AlertMessage";
        private const string AlertTypeKey = "AlertType";

        public static void setAlert(this ITempDataDictionary tempData, string message, AlertTypes type)
        {
            tempData[AlertMessageKey] = message;
            tempData[AlertTypeKey] = type.ToString();

        }

        public static (string Message, string Type)? GetAlert(this ITempDataDictionary tempData )
        {
            if(tempData.ContainsKey(AlertMessageKey) && tempData.ContainsKey(AlertTypeKey))
            {
                return (tempData[AlertMessageKey]?.ToString(),
                        tempData[AlertTypeKey]?.ToString());
            }
            return null;    
                    }
            }
}
