using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using sistem_e_daftar_gemaputera.Models;

[Authorize]
public class PasukanController : Controller
{
    private readonly ApplicationDbContext _Db;

    public PasukanController(ApplicationDbContext applicationDbContext)
    {
        _Db = applicationDbContext;
    }



    public async Task<IActionResult> Index()
    {
        ViewBag.SenaraiSukan = await _Db.Sukan.AsNoTracking().Select(x => x.Nama).ToListAsync();
        return View();
    }

    public async Task<IActionResult> GetPasukan(string jenisSukan)
    {
        var _agensi         = User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).Single();
        var _ListOfAhli     = await _Db.Ahli.AsNoTracking().Where(x => x.JenisSukan == jenisSukan && x.Agensi == _agensi).ToListAsync();
        var _Configuration  = await _Db.Sukan.AsNoTracking().Where(x => x.Nama == jenisSukan).FirstAsync();

        var _model = new List<Ahli>();

        if (_ListOfAhli.Count == 0)
        {
            //Pengurus
            for (int i = 0; i < _Configuration.BilanganJenisAhli.Pengurus; i++)
            {
                _model.Add(new Ahli { JenisAhli = "Pengurus", JenisSukan = jenisSukan, Agensi = _agensi });
            }

             //Jurulatih
            for (int i = 0; i < _Configuration.BilanganJenisAhli.Jurulatih; i++)
            {
                _model.Add(new Ahli { JenisAhli = "Jurulatih", JenisSukan = jenisSukan, Agensi = _agensi });
            }

             //Fisio
            for (int i = 0; i < _Configuration.BilanganJenisAhli.Fisio; i++)
            {
                _model.Add(new Ahli { JenisAhli = "Fisio", JenisSukan = jenisSukan, Agensi = _agensi });
            }

            //Pemain
            for (int i = 0; i < _Configuration.BilanganJenisAhli.Pemain; i++)
            {
                _model.Add(new Ahli { JenisAhli = "Pemain", JenisSukan = jenisSukan, Agensi = _agensi });
            }

            _Db.UpdateRange(_model);
            
            await _Db.SaveChangesAsync();
        }
        else
        {
            _model = _ListOfAhli;
        }

        return Json(new { pasukan = _model, kategori = _Configuration.KategoryAsList });

    }

    [HttpPost]
    public async Task<IActionResult> SetPasukan(List<Ahli> data)
    {
        var _result = false;

        try
        {
            _Db.UpdateRange(data);
            await _Db.SaveChangesAsync();

            _result = true;

            await Helper.Log( _Db, User.Identity?.Name ?? "", "PasukanController/SetPasukan");
        }
        catch (Exception err)        
        {
            await Helper.Log( _Db, err.Message, "PasukanController/SetPasukan");    
        }

        return new JsonResult(new { status = _result });
    }

    [HttpPost]
    public async Task<IActionResult> DelPasukan(List<Ahli> data)
    {
        var _result = false;

        try
        {
            _Db.RemoveRange(data);
            await _Db.SaveChangesAsync();

            _result = true;
            await Helper.Log( _Db, User.Identity?.Name ?? "", "PasukanController/DelPasukan");
        }
        catch (Exception err)
        {
            await Helper.Log( _Db, err.Message, "PasukanController/DelPasukan");
        }
        
        return new JsonResult(new { status = _result });
    }


    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] string data)
    {

        try
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
        catch
        {
            return new JsonResult(new { status = false });
        }

    }

    public async Task<IActionResult> Cetak(string sukan, string? agensi)
    {
        if (agensi == null)
        {
            agensi = User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).Single();
        }
        //var _agensi = User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).Single();

        var _model = await _Db.Ahli.AsNoTracking().Where(x => x.JenisSukan == sukan && x.Agensi == agensi).ToListAsync();

        ViewBag.TarikhTutup = await _Db.Agensi.AsNoTracking().Where(x => x.Nama == agensi).Select(x => x.TarikhTutup.ToString("dd/MMM/yyyy")).SingleAsync();


        return View(_model);
    }

}