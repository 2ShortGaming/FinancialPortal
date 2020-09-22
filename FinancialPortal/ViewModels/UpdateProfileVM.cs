using FinancialPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Web;

namespace FinancialPortal.ViewModels
{
    public class UpdateProfileVM
    {
        public string Id { get; set; }
        [Display(Name = "First Name")]
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First Name must be between 2 and 50 characters")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last Name must be between 2 and 50 characters")]
        public string LastName { get; set; }
        public string AvatarPath { get; set; }

        public UpdateProfileVM(ApplicationUser user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            
        }
        public UpdateProfileVM()
        {

        }
    }
}