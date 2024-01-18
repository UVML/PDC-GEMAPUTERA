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
        var _pengguna = await _Db.Pengguna.AsNoTracking().Where(x=>string.IsNullOrEmpty(x.FileYuran)==false).ToListAsync();
        var _ListOfAgensi = _pengguna.Select(x => x.Agensi).ToList();

        var _ahli     = await _Db.Ahli.AsNoTracking().Where(x => _ListOfAgensi.Contains(x.Agensi)).ToListAsync();        
        var _sukan    = await _Db.Sukan.AsNoTracking().ToListAsync();

        var _agencies = _ahli.GroupBy(a => a.Agensi).Select(b => new
        {
            Name       = b.Key,            
            Fee        = _sukan.Where(s => b.Any(d => d.JenisSukan == s.Nama)).Sum(e => e.Fee),
            FileYuran  = _pengguna.Where(p => p.Agensi == b.Key).FirstOrDefault()?.FileYuran, 
            FileResit  = _pengguna.Where(p => p.Agensi == b.Key).FirstOrDefault()?.FileResit,
            TarikhHantar = _pengguna.Where(p => p.Agensi == b.Key).FirstOrDefault()?.TarikhHantar,
            TarikhResit = _pengguna.Where(p => p.Agensi == b.Key).FirstOrDefault()?.TarikhResit,
        }).ToList();


        ViewBag.Agencies = _agencies;

        return View(_ahli);
    }

    public async Task<IActionResult> GetPengguna()
    {
        var _model = await _Db.Pengguna.AsNoTracking().ToListAsync();
        return Json(_model);
    }

    public async Task<IActionResult> PadamPengguna(string Agensi)
    {
        try
        {
            var _record = await _Db.Pengguna.SingleAsync(x => x.Agensi == Agensi);
            _Db.Pengguna.Remove(_record);

            await _Db.SaveChangesAsync();
            return Json(new { status = true });
        }
        catch
        {
            return Json(new { status = false });
        }
    }

    public async Task<IActionResult> GetSukan()
    {
        var _model = await _Db.Sukan.AsNoTracking().ToListAsync();
        return Json(_model);
    }

     public async Task<IActionResult> SimpanSukan(List<Sukan> sukan)
    {
        try
        {
            _Db.UpdateRange(sukan);
            await _Db.SaveChangesAsync();
            
            return Json(new { status = true });
        }
        catch
        {
            return Json(new { status = false });
        }
    }

    public async Task<IActionResult> GetTarikhTutup()
    {
        var _model =_Db.Setting.AsNoTracking().Single(x=>x.Key=="TarikhTutup").Value;
        
        if(DateTime.TryParse(_model, out DateTime _date))
        {
            _model = _date.ToString("yyyy-MM-dd");
        }
        else
        {
            _model = DateTime.Now.ToString("yyyy-MM-dd");
        }

        return Json(_model);
    }

     public async Task<IActionResult> SimpanTarikhTutup(string tarikhTutup)
    {
        try
        {
            var _item = await _Db.Setting.SingleAsync(x=>x.Key=="TarikhTutup");
            _item.Value = tarikhTutup;
            
            _Db.Update(_item);
            await _Db.SaveChangesAsync();
            
            return Json(new { status = true });
        }
        catch
        {
            return Json(new { status = false });
        }
    }

    [HttpPost]
    public async Task<IActionResult> UploadResit(IFormFile file, string agensi)
    {
        try
        {
            
            var _folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", agensi);

            if (!Directory.Exists(_folder))
            {
                Directory.CreateDirectory(_folder);
            }

            var _filePath = Path.Combine(_folder, file.FileName);
            
            var _pengguna = await _Db.Pengguna.SingleAsync(x => x.Agensi == agensi);
            _pengguna.FileResit = file.FileName;
            _pengguna.TarikhResit = DateTime.Now;
            
            await _Db.SaveChangesAsync();


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

}