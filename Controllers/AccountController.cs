using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecruitApp.Models;
using RecruitApp.ViewModels;

namespace RecruitApp.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Identifiants invalides.");
            return View(model);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Recruteur") && !user.IsValidated)
            {
                ModelState.AddModelError(string.Empty, "Votre compte recruteur doit être validé par un admin avant la connexion.");
                return View(model);
            }

            await _signInManager.SignInAsync(user, model.RememberMe);
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Identifiants invalides.");
        return View(model);
    }

    [HttpGet]
    public IActionResult Register(AccountType type = AccountType.Candidat) => View(new RegisterViewModel { AccountType = type });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser is not null)
        {
            ModelState.AddModelError(nameof(model.Email), "Un compte existe déjà avec cet email.");
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            Prenom = model.Prenom,
            Nom = model.Nom,
            CompanyName = model.AccountType == AccountType.Recruteur ? model.CompanyName : null,
            CompanyDomain = model.AccountType == AccountType.Recruteur ? model.CompanyDomain : null,
            CompanyWebsite = model.AccountType == AccountType.Recruteur ? model.CompanyWebsite : null,
            CompanyAddress = model.AccountType == AccountType.Recruteur ? model.CompanyAddress : null,
            CompanyDescription = model.AccountType == AccountType.Recruteur ? model.CompanyDescription : null,
            CreatedAt = DateTime.UtcNow,
            IsValidated = model.AccountType == AccountType.Candidat
        };

        var createResult = await _userManager.CreateAsync(user, model.Password);
        if (!createResult.Succeeded)
        {
            foreach (var error in createResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        var roleName = model.AccountType == AccountType.Recruteur ? "Recruteur" : "Candidat";
        if (await _roleManager.RoleExistsAsync(roleName))
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        TempData["SuccessMessage"] = model.AccountType == AccountType.Recruteur
            ? "Compte recruteur créé. Il sera actif après validation par un admin."
            : "Compte candidat créé avec succès. Vous pouvez vous connecter.";
        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }
}
