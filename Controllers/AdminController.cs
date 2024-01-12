using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sistem_e_daftar_gemaputera.Models;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _Db;

    public AdminController(ApplicationDbContext applicationDbContext)
    {
        _Db = applicationDbContext;
    }

    private async Task<IActionResult> Index()
    {
        return View();
    }


    
}