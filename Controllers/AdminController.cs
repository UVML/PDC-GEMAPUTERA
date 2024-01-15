using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sistem_e_daftar_gemaputera.Models;

[Authorize(Roles = "Administrator")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _Db;

    public AdminController(ApplicationDbContext applicationDbContext)
    {
        _Db = applicationDbContext;
    }

    public async Task<IActionResult> Index()
    {
        var _ahli      = await _Db.Ahli.AsNoTracking().ToListAsync();
        var _sukan     = await _Db.Sukan.AsNoTracking().ToListAsync();

         var _agencies = _ahli.GroupBy(a=>a.Agensi).Select(b=> new
         {
            Name    = b.Key,
            IsResit = Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", b.Key)),
            File    = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", b.Key)).FirstOrDefault()?.Split("\\").LastOrDefault()?.ToLower(),
            Fee     = _sukan.Where(s=>b.Any(d=>d.JenisSukan==s.Nama)).Sum(e=>e.Fee)
         }).ToList();
        
        
        ViewBag.Agencies = _agencies;
        
        return View(_ahli);
    }


    
}