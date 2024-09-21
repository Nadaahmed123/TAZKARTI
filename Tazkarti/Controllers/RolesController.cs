using Tazkarti.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Tazkarti.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<RolesController> _logger;

        public RolesController(
            UserManager<IdentityUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<RolesController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        public IActionResult Create()
        {
            return View(new ApplicationRole());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationRole role)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                    return RedirectToAction("Index");

                foreach (var error in result.Errors)
                {
                    _logger.LogError(error.Description);
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(role);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
                return NotFound();

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();

            return View(role);
        }

        public async Task<IActionResult> Update(string id)
        {
            return await Details(id);
        }

        [HttpPost]
        public async Task<IActionResult> Update(string id, ApplicationRole applicationRole)
        {
            if (id != applicationRole.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var role = await _roleManager.FindByIdAsync(id);
                    if (role == null)
                        return NotFound();

                    role.Name = applicationRole.Name;
                    role.NormalizedName = applicationRole.Name.ToUpper();

                    var result = await _roleManager.UpdateAsync(role);
                    if (result.Succeeded)
                        return RedirectToAction("Index");

                    foreach (var error in result.Errors)
                    {
                        _logger.LogError(error.Description);
                        ModelState.AddModelError("", error.Description);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    ModelState.AddModelError("", "An error occurred while updating the role.");
                }
            }

            return View(applicationRole);
        }

        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                    return NotFound();

                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                    return RedirectToAction("Index");

                foreach (var error in result.Errors)
                {
                    _logger.LogError(error.Description);
                    ModelState.AddModelError("", error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> AddOrRemoveUsers(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound();

            ViewBag.RoleId = roleId;

            var users = await _userManager.Users.ToListAsync(); // Await here

            var usersInRole = new List<UserInRoleViewModel>();

            foreach (var user in users)
            {
                var userInRole = new UserInRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                    userInRole.IsSelected = true;
                else
                    userInRole.IsSelected = false;

                usersInRole.Add(userInRole);
            }

            return View(usersInRole);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrRemoveUsers(List<UserInRoleViewModel> users, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                foreach (var item in users)
                {
                    var user = await _userManager.FindByIdAsync(item.UserId);

                    if (user != null)
                    {
                        if (item.IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                            await _userManager.AddToRoleAsync(user, role.Name);
                        else if (!item.IsSelected && (await _userManager.IsInRoleAsync(user, role.Name)))
                            await _userManager.RemoveFromRoleAsync(user, role.Name);
                    }
                }
                return RedirectToAction(nameof(Update), new { id = roleId });
            }

            return View(users);
        }



    }
}

