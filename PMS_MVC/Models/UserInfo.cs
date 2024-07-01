﻿using System.ComponentModel.DataAnnotations;

namespace PMS_MVC.Models
{
    public class UserInfo
    {
        [Required(ErrorMessage = "Email is required")]
        //[RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail address")]
        [EmailAddress(ErrorMessage ="Please enter a valide e-mail address")]
        [StringLength(50, ErrorMessage = "Only 50 Characaters are Accepted")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contains one Uppercase, one Lowercase and Special character")]
        [StringLength(50, ErrorMessage = "Only 50 Characaters are Accepted")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Your password and confirmation password do not match.")]
        [StringLength(50, ErrorMessage = "Only 50 Characaters are Accepted")]
        public string? ConfirmPassword { get; set; }
    }
}
