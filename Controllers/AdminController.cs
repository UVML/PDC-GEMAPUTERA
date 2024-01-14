using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sistem_e_daftar_gemaputera.Models;

[Authorize]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _Db;

    public AdminController(ApplicationDbContext applicationDbContext)
    {
        _Db = applicationDbContext;
    }

    public async Task<IActionResult> Index()
    {
        var _agensy = _Db.Ahli.AsNoTracking().Select(x=>x.Agensi).Distinct().ToList();

         var _agencies = _agensy.Select(x=> new
         {
            Name= x,
            IsResit = Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", x)),
            File = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", x)).FirstOrDefault()?.Split("\\").LastOrDefault()?.ToLower() 
         }).ToList();
        
        var _AgenciesToShow = _agencies.Where(x=>x.IsResit).Select(x=>x.Name).ToList();
        var _ahli   = await _Db.Ahli.AsNoTracking().Where(x=>_AgenciesToShow.Contains(x.Agensi)).ToListAsync();
        var _sukan  = await _Db.Sukan.AsNoTracking().ToListAsync();
        var _model  = _ahli.GroupBy(x => x.JenisSukan).Select(x => new
        {
            Sukan = x.Key,
            Fee = _sukan.Where(y => y.Nama == x.Key).Single().Fee,
            Agensi = x.FirstOrDefault()?.Agensi,
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
        
        ViewBag.Agencies = _agencies;
        
        return View(_model);
    }


    
}