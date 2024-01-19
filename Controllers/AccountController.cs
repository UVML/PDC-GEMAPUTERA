using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using MimeKit;
using Newtonsoft.Json;
using sistem_e_daftar_gemaputera.Models;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _Db;    
    private readonly IHttpClientFactory _httpClientFactory;

    public AccountController(ApplicationDbContext applicationDbContext, IHttpClientFactory httpClientFactory)
    {
        _Db = applicationDbContext;        
        _httpClientFactory = httpClientFactory;
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
        var _url = Url.Content("~/Home/Index");
        var _result = false;

        if (_query != null && _query.KataLaluan == password)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, _query.Emel),
                new Claim(ClaimTypes.Role, _query.Agensi)
            };

            var _identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var _principal = new ClaimsPrincipal(_identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, _principal, new AuthenticationProperties { IsPersistent = false });

            if (email.ToLower() == "administrator")
            {
                _url = Url.Content("~/Admin/Index");
            }

            _result = true;
        }
    
        return new JsonResult(new { status = _result, url = _url });

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
        var _result   = false;
        var _tac_code = HttpContext.Session.GetString("tac_code");
        var _expiry   = HttpContext.Session.GetString("tac_expiry");

        Console.WriteLine(_tac_code);
        Console.WriteLine(_expiry);
        Console.WriteLine(data.TAC);

        if (_tac_code == data.TAC && DateTime.Now < DateTime.Parse(_expiry))
        {
            
            try
            {
                Console.WriteLine(data.Agensi);
                await _Db.AddAsync(data);
                await _Db.SaveChangesAsync();

                _result = true;
            }
            catch (Exception err)
            {
                await Helper.Log(_Db, err.Message, "AccountController/Daftar");
            }

        }
                
        return new JsonResult(new { status = _result });


    }


    [HttpPost]
    public async Task<IActionResult> setTAC(string msisdn)
    {

        var _tac    = new Random().Next(1000, 9999);
        var _result = false;

        var _setting = await _Db.Setting.AsNoTracking().SingleOrDefaultAsync(x => x.Key == "SMS");
        var _sms     = JsonConvert.DeserializeObject<Setting_SMS>(_setting.Value);
        
        if (_sms != null)
        {
            try
            {
                var _payload = new { keyword= "NIOSH", msisdn = msisdn, message = "Pendaftaran Gemaputera 2024. Kod TAC anda adalah: " + _tac };
                var _client  = _httpClientFactory.CreateClient();
                var _content = new StringContent(JsonConvert.SerializeObject(_payload), Encoding.UTF8, "application/json");
                // _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(_sms.API)));
                
                _client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", _sms.API);
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var _response = await _client.PostAsync(_sms.Url, _content);

                var _responseContent = await _response.Content.ReadAsStringAsync();
                if (_response.IsSuccessStatusCode)
                {
                    HttpContext.Session.SetString("tac_code", _tac.ToString());
                    HttpContext.Session.SetString("tac_expiry", DateTime.Now.AddMinutes(5).ToString());
                    _result = true;
                }
                else
                {
                    await Helper.Log(_Db, _responseContent, "AccountController/setTAC");
                }
            }
            catch (Exception err)
            {
                await Helper.Log(_Db, err.Message, "AccountController/setTAC");
            }
            
        }

        return new JsonResult(new { status = _result});
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
                await Helper.Log(_Db, err.Message, "AccountController/ForgotPassword");
            }
        }

        return new JsonResult(new { status = _result });

    }

}