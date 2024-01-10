using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sistem_e_daftar_gemaputera.Models;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _Db;

    public AccountController(ApplicationDbContext applicationDbContext)
    {
        _Db = applicationDbContext;
    }

    public async Task<ActionResult> Login()
    {
        var _registeredAgencies = _Db.Users.AsNoTracking().Select(x=>x.Agency).Distinct();
        ViewBag.Agencies        = await _Db.Agencies.AsNoTracking().Select(x=>x.Name).Except(_registeredAgencies).ToListAsync();

        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<ActionResult> Login(string email, string password)
    {
        var _query = await _Db.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Email == email);

        if (_query != null && _query.Password == password)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, _query.Email)                
            };

            var _identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var _principal = new ClaimsPrincipal(_identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, _principal);

            return RedirectToAction("Index", "Home");
        }
        else
        {
            return View();
        }

    }

    [HttpPost]
    public async Task<ActionResult> Register(User user)
    {
        try
        {
            await _Db.AddAsync(user);
            await _Db.SaveChangesAsync();
            return RedirectToAction("Login");
        }
        catch
        {
            return View("Login", user);
        }

    }

   
}