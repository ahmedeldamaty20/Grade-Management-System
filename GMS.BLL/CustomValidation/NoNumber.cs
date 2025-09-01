using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GMS.BLL.CustomValidation
{
    public class NoNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string input = value.ToString();

                if (Regex.IsMatch(input, @"\d"))
                {
                    return new ValidationResult("Course name cannot contain numbers.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
