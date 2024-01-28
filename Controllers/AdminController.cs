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
        var _sukan    = await _Db.Sukan.AsNoTracking().ToListAsync();
        var _ahli     = await _Db.Ahli.AsNoTracking().ToListAsync();

        var _model    = _pengguna.OrderBy(x=>x.Agensi).Select(x=>new {
            Agensi    = x.Agensi,
            FileYuran = x.FileYuran,
            FileResit = x.FileResit,
            TarikhHantar = x.TarikhHantar?.ToString("dd/MM/yyyy"),
            TarikhResit = x.TarikhResit?.ToString("dd/MM/yyyy"),
            Catatan   = x.Catatan,
            Ahlis     = _ahli.Where(y=>y.Agensi == x.Agensi).ToList(),
            Yuran     = _sukan.Where(y=> _ahli.Where(z=>z.Agensi == x.Agensi).Select(z=>z.JenisSukan).Contains(y.Nama)).Sum(y=>y.Fee).ToString("#,##0.00"),
            Sukans    = _ahli.Where(y=>y.Agensi == x.Agensi).Select(y=>y.JenisSukan).Distinct()
        });

        //⚠️ORDER IS IMPORTANT - To ensure the index ordinal of the array is correct
        ViewBag.Attachment = _model.Select(x=>new {x.Agensi, x.FileYuran, x.FileResit, x.TarikhHantar, x.TarikhResit, x.Catatan});
        return View(_model);
    }

    public async Task<IActionResult> GetPengguna()
    {
        var _model = await _Db.Pengguna.AsNoTracking().Where(x=>x.Agensi != "Administrator").ToListAsync();
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

    public async Task<IActionResult> GetAgensi()
    {
        var _model = await _Db.Agensi.AsNoTracking().ToListAsync();
        return Json(_model);
    }

    [HttpPost]
     public async Task<IActionResult> SimpanAgensi(List<Agensi> data)
    {
        try
        {
            _Db.UpdateRange(data);
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

    public async Task<IActionResult> GetAttachment()
    {
        //⚠️ORDER IS IMPORTANT - To ensure the index ordinal of the array is correct
        var _pengguna = await _Db.Pengguna.AsNoTracking().OrderBy(x=>x.Agensi).ToListAsync();

        return new JsonResult(_pengguna.Select(x=>new 
        {
            x.Agensi, 
            x.FileYuran, 
            x.FileResit, 
            TarikhHantar = x.TarikhHantar?.ToString("dd/MM/yyyy"),
            TarikhResit = x.TarikhResit?.ToString("dd/MM/yyyy"), 
            x.Catatan
        }));
    }

}