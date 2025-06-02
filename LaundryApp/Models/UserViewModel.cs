// Models/UserViewModel.cs (atau ViewModels/UserViewModel.cs)
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LaundryApp.Models // atau LaundryApp.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Username")]
        public string? UserName { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Peran (Roles)")]
        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}