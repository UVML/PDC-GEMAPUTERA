using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Newtonsoft.Json;
using sistem_e_daftar_gemaputera.Models;


namespace sistem_e_daftar_gemaputera.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _Db;
    
    private readonly IConfiguration _Configuration;

    public HomeController(ApplicationDbContext applicationDbContext, IConfiguration configuration)
    {
        _Db = applicationDbContext;   
        _Configuration = configuration;     
    }
    
    

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var _agensi = User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).Single();
        var _ahli = await _Db.Ahli.AsNoTracking().Where(x => x.Agensi == _agensi).ToListAsync();
        var _sukan = await _Db.Sukan.AsNoTracking().ToListAsync();

        var _model = _ahli.GroupBy(x => x.JenisSukan).Select(x => new
        {
            Sukan = x.Key,
            Fee = _sukan.Where(y => y.Nama == x.Key).Single().Fee,
            Pengurus = x.Where(y => y.JenisAhli == "Pengurus").All(x=>x.IsCompleted),
            Jurulatih = x.Where(y => y.JenisAhli == "Jurulatih").FirstOrDefault()?.IsCompleted,
            Fisio = _sukan.Where(y => y.Nama == x.Key).Single().KonfigurasiAhli.Single(y => y.JenisAhli == "Fisio").Size > 0 ? x.Where(y => y.JenisAhli == "Fisio").All(z=>z.IsCompleted): true,
            Pemain = x.Where(y => y.JenisAhli == "Pemain" & y.IsCompleted).Count(),
            Bilangan = x.Where(y => y.JenisAhli == "Pemain").Count(),

            Pengurus_Gambar = x.Where(y => y.JenisAhli == "Pengurus").FirstOrDefault()?.FileGambar,
            Pengurus_Id = x.Where(y => y.JenisAhli == "Pengurus").FirstOrDefault()?.Id,

            Jurulatih_Gambar = x.Where(y => y.JenisAhli == "Jurulatih").FirstOrDefault()?.FileGambar,
            Jurulatih_Id = x.Where(y => y.JenisAhli == "Jurulatih").FirstOrDefault()?.Id,
        }).ToList();

        ViewBag.TotalFee = _model.Sum(x => x.Fee).ToString("#,##0.00");
        ViewBag.Agensi = _agensi;
        

        ViewBag.Bank = JsonConvert.DeserializeObject(_Db.Setting.AsNoTracking().Where(x => x.Key == "Bank").Select(x => x.Value).Single());

        return View(_model);
    }

    [HttpPost]
    public async Task<IActionResult> UploadResit(IFormFile file)
    {

        try
        {
            var _agensi = User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).Single();

            var _folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", _agensi);

            if (!Directory.Exists(_folder))
            {
                Directory.CreateDirectory(_folder);
            }

            var _filePath = Path.Combine(_folder, file.FileName);

            using (var stream = new FileStream(_filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return new JsonResult(new { status = true });
        }
        catch
        {
            return new JsonResult(new { status = false });
        }


    }

    public async Task<IActionResult> GetResit()
    {
        try
        {
            var _agensi = User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).Single();
            var _folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", _agensi);

            if (!Directory.Exists(_folder))
            {
                Directory.CreateDirectory(_folder);
            }

            var _file = Directory.GetFiles(_folder)[0];

            return Json(new { status = true, resit = Path.GetFileName(_file) });
        }
        catch
        {
            return Json(new { status = false });
        }
    }


    public async Task<IActionResult> TukarKataLaluan(string KataLaluanLama, string KataLaluanBaru)
    {
        var _result = false;
        var _user   = await _Db.Pengguna.Where(x => x.Emel == User.Identity.Name).SingleAsync();

        if (_user.KataLaluan == KataLaluanLama)        
        {
            _user.KataLaluan = KataLaluanBaru;
            await _Db.SaveChangesAsync();     
            
            _result = true;       
        }

        return Json(new { status = _result });
    }


   
}
