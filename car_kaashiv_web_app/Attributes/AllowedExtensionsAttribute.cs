using Humanizer;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;


namespace car_kaashiv_web_app.Attributes
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;
        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if(file == null)
            {
                // if no file uploaded, don't block model
                return ValidationResult.Success;
            }
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_extensions.Contains(extension))
            {
                return new ValidationResult($"Only {string.Join(",", _extensions)} files are allowed");
            }

            if (file.Length > 2 * 1024 * 1024)
            {
                return new ValidationResult("Maximum allowed file size is 2 MB");
            }

            return ValidationResult.Success;
        }
    }
}
