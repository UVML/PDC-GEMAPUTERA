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

    private async Task<List<string>> GetAgencies()
    {
        var _registeredAgencies = _Db.Pengguna.AsNoTracking().Select(x=>x.Agensi).Distinct();
        return await _Db.Agensi.AsNoTracking().Select(x=>x.Nama).Except(_registeredAgencies).ToListAsync();
    }


    public async Task<ActionResult> Login()
    {
        
        ViewBag.Agencies = await GetAgencies();
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
        var _query = await _Db.Pengguna.AsNoTracking().SingleOrDefaultAsync(x => x.Emel == email);

        if (_query != null && _query.KataLaluan == password)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, _query.Emel),
                new Claim(ClaimTypes.Role, _query.Agensi)          
            };

            var _identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var _principal = new ClaimsPrincipal(_identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, _principal);
            
            if (email.ToLower() == "administrator")
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }
        else
        {
            ViewBag.Agencies = await GetAgencies();
            return View();
        }

    }

    [HttpPost]
    public async Task<ActionResult> Register(Pengguna pengguna)
    {
        try
        {
            await _Db.AddAsync(pengguna);
            await _Db.SaveChangesAsync();

            return RedirectToAction("Login");
        }
        catch
        {
            return View("Login", pengguna);
        }

    }

}