using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using sistem_e_daftar_gemaputera.Models;


namespace sistem_e_daftar_gemaputera.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _Db;

    public HomeController(ApplicationDbContext applicationDbContext)
    {
        _Db = applicationDbContext;
    }


    [Authorize]
    public async Task<IActionResult> Index()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Pasukan()
    {
        ViewBag.SenaraiSukan = await _Db.Sukan.AsNoTracking().Select(x=>x.Nama).ToListAsync();
        return View();
    }

    public async Task<IActionResult> GetPasukan(string jenisSukan)
    {
        var _agensi        = User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).Single();

        var _ListOfAhli    = await _Db.Ahli.AsNoTracking().Where(x => x.JenisSukan == jenisSukan && x.Agensi==_agensi).ToListAsync();
        var _Configuration = await _Db.Sukan.AsNoTracking().Where(x => x.Nama == jenisSukan).FirstAsync();

        var _model         = new List<Ahli>();
        _model.AddRange(_ListOfAhli);

        foreach(var _config in _Configuration.KonfigurasiAhli)
        {            
            var _recordSize = _ListOfAhli.Count(x=>x.JenisAhli == _config.JenisAhli);
            if (_config.Size != _recordSize )
            {
                for (int i = 0; i < _config.Size - _ListOfAhli.Count(x => x.JenisAhli == _config.JenisAhli); i++)
                {                    
                    _model.Add(new Ahli { JenisAhli = _config.JenisAhli, JenisSukan = jenisSukan, Agensi = _agensi });
                }                
            }
        }

        return Json(_model);

    }

    [HttpPost]
    public async Task<IActionResult> SetPasukan(List<Ahli> data)
    {
        _Db.UpdateRange(data);
        await _Db.SaveChangesAsync();

        return new JsonResult(new { status = true });
    }

    

    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] string data)
    {
        
        var _ahli = JsonConvert.DeserializeObject<Ahli>(data);
        
        var _folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", _ahli.Id.ToString());
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


}
