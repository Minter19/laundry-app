// Controllers/UserManagementController.cs
using Microsoft.AspNetCore.Authorization; // Untuk [Authorize]
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LaundryApp.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Microsoft.EntityFrameworkCore;
namespace LaundryApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Menampilkan form pendaftaran
        public IActionResult RegisterUserByAdmin()
        {
            // Siapkan daftar peran yang bisa dipilih (misalnya "Operator", "Guest")
            // Anda bisa filter peran apa saja yang boleh dibuat oleh admin di sini
            var roles = _roleManager.Roles
                            .Select(r => new SelectListItem
                            {
                                Value = r.Name,
                                Text = r.Name
                            }).ToList();

            var model = new AdminRegisterUserViewModel
            {
                AvailableRoles = roles
            };
            return View(model);
        }

        // POST: Memproses data dari form pendaftaran
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUserByAdmin(AdminRegisterUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Jika model tidak valid, kirim kembali daftar peran ke view
                model.AvailableRoles = _roleManager.Roles
                                        .Select(r => new SelectListItem
                                        {
                                            Value = r.Name,
                                            Text = r.Name
                                        }).ToList();
                return View(model);
            }

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Jika user berhasil dibuat, tambahkan ke peran yang dipilih
                if (!string.IsNullOrEmpty(model.SelectedRole) && await _roleManager.RoleExistsAsync(model.SelectedRole))
                {
                    await _userManager.AddToRoleAsync(user, model.SelectedRole);
                }
                // else: handle jika peran tidak dipilih atau tidak valid

                TempData["SuccessMessage"] = $"Pengguna {user.Email} berhasil didaftarkan dengan peran {model.SelectedRole}.";
                return RedirectToAction("ListOfUsers", "UserManagement");

            }

            // Jika gagal, kirim kembali daftar peran dan tampilkan error
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            model.AvailableRoles = _roleManager.Roles
                                    .Select(r => new SelectListItem
                                    {
                                        Value = r.Name,
                                        Text = r.Name
                                    }).ToList();
            return View(model);
        }

        [HttpGet] // Hanya untuk GET request
        [Authorize(Roles = "Admin")] // Pastikan hanya Admin yang bisa akses
        public async Task<IActionResult> ListOfUsers()
        {
            var users = await _userManager.Users.ToListAsync(); // Ambil semua user
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user); // Dapatkan peran untuk setiap user
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles
                });
            }

            // Pesan sukses dari TempData akan otomatis tersedia di View jika ada
            return View(userViewModels);
        }
    }
}
