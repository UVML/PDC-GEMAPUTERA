using System.Security.Claims;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Newtonsoft.Json;
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
        var _registeredAgencies = _Db.Pengguna.AsNoTracking().Select(x => x.Agensi).Distinct();
        return await _Db.Agensi.AsNoTracking().Select(x => x.Nama).Except(_registeredAgencies).ToListAsync();
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

            var _identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
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

    [HttpPost]
    public async Task<IActionResult> Daftar(Pengguna data)
    {
        try
        {
            Console.WriteLine(data.Agensi);
            await _Db.AddAsync(data);
            await _Db.SaveChangesAsync();

            return new JsonResult(new { status = true });
        }
        catch (Exception err)
        {
            Console.WriteLine(err.Message);

            return new JsonResult(new { status = false });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        var _pengguna = await _Db.Pengguna.AsNoTracking().SingleOrDefaultAsync(x => x.Emel == email);
        var _setting = await _Db.Setting.AsNoTracking().SingleAsync(x => x.Key == "SMTP");
        var _smtp = JsonConvert.DeserializeObject<Setting_SMTP>(_setting.Value);

        var _result = false;

        if (_smtp != null)
        {
            try
            {
                var _message     = new MimeMessage();

                _message.From.Add(new MailboxAddress( "Sistem Pendaftaran Gamaputera Pulau Pinang", _smtp.From));
                _message.To.Add(new MailboxAddress(_pengguna?.Emel, _pengguna?.Emel));
                _message.Subject = "Lupa Kata Laluan";
                _message.Body    = new TextPart("plain"){Text = "Kata Laluan anda adalah " + _pengguna?.KataLaluan};

                using (var _client = new SmtpClient())
                {
                    _client.Connect(_smtp.Host, _smtp.Port, _smtp.UseSSL);
                    _client.Authenticate(_smtp.Username, _smtp.Password);

                    await _client.SendAsync(_message);
                    _client.Disconnect(true);
                }
                _result = true;
            }
            catch (Exception err)
            {
                Helper.Log(_Db, err.Message, "AccountController/ForgotPassword");
            }
        }

        return new JsonResult(new { status = _result });

    }

}