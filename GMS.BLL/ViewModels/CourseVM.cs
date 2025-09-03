using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.AspNetCore.Mvc;

using GMS.DAL.Models;
using GMS.BLL.CustomValidation;

namespace GMS.BLL.ViewModels
{
    public class CourseVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Course name is required")]
        [NoNumber]
        [Remote(action: "NameExistVal", controller: "Course", ErrorMessage = "Course name is already taken",AdditionalFields =nameof(Id))]
        [MinLength(3, ErrorMessage = "Course name must be at least 3 characters long")]
        [MaxLength(50, ErrorMessage = "Course name cannot exceed 50 characters")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Category is required")]
        public CourseCategory Category { get; set; }
        public string? InstructorId { get; set; }
        public int selectedCategoryValue { get; set; }

    }
}