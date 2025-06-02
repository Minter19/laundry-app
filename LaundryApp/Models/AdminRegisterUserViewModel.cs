// Models/AdminRegisterUserViewModel.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace LaundryApp.Models
{
    public class AdminRegisterUserViewModel
    {
        [Required(ErrorMessage = "Email wajib diisi.")]
        [EmailAddress]
        [Display(Name = "Email Calon Pengguna")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password wajib diisi.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password Sementara")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Konfirmasi Password")]
        [Compare("Password", ErrorMessage = "Password dan konfirmasi password tidak cocok.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Peran wajib dipilih.")]
        [Display(Name = "Peran Pengguna")]
        public string SelectedRole { get; set; } // Untuk menampung role yang dipilih

        public IEnumerable<SelectListItem>? AvailableRoles { get; set; }
    }
}