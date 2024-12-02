using KeyMaster_MVC.Models;
using KeyMaster_MVC.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Drawing2D;

namespace KeyMaster_MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/User")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dataContext;

        public UserController(DataContext context,UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dataContext = context;
        }
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var usersWithRoles = await (from u in _dataContext.Users
                                        join ur in _dataContext.UserRoles on u.Id equals ur.UserId
                                        join r in _dataContext.Roles on ur.RoleId equals r.Id
                                        select new { User = u, RoleName = r.Name }).ToListAsync();
            return View(usersWithRoles);
        }
        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(new AppUserModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(AppUserModel user)
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            if (ModelState.IsValid)
            {
                var createUserResult = await _userManager.CreateAsync(user, user.PasswordHash);
                if (createUserResult.Succeeded)
                {
                    var createUser = await _userManager.FindByEmailAsync(user.Email); // tìm user dựa vào email
                    var userId = createUser.Id; // lấy user Id
                    var role = await _roleManager.FindByIdAsync(user.RoleId); // tìm role dựa vào RoleId
                    if (role == null)
                    {
                        ModelState.AddModelError(string.Empty, "Role không hợp lệ.");
                        return View(user);
                    }

                    // gán quyền cho user
                    var addToRoleResult = await _userManager.AddToRoleAsync(createUser, role.Name);
                    if (!addToRoleResult.Succeeded)
                    {
                        foreach (var error in addToRoleResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(user);
                    }

                    return RedirectToAction("Index", "User");
                }
                else
                {
                    foreach (var error in createUserResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(user);
                }
            }
            else
            {
                TempData["error"] = "Thêm user không thành công";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }
        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public async Task<IActionResult> Edit(string Id, AppUserModel user)
        {
            var existingUser = await _userManager.FindByIdAsync(Id); // lấy user dựa vào Id
            if (existingUser == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Update other user properties (excluding password)
                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.RoleId = user.RoleId;

                var updateUserResult = await _userManager.UpdateAsync(existingUser);
                if (updateUserResult.Succeeded)
                {
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    foreach (var error in updateUserResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(existingUser);
                }
            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            //Model validation failed 
            TempData["error"] = "Cập nhật user không thành công";
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage).ToList());
            string errorMessage = string.Join("\n", errors);
            return View(existingUser);
        }

        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return NotFound();
            }
            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                return View("Error");
            }
            TempData["success"] = "User đã được xóa thành công";
            return RedirectToAction("Index");
        }
    }
}

