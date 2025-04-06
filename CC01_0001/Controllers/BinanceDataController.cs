/*
    Controllers/BinanceDataController.cs
    Version: 1.0.0
    (c) 2024, Minh Tri Tran, with assistance from Google's Gemini - Licensed under CC BY 4.0
    https://creativecommons.org/licenses/by/4.0/
*/
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection; // Add this using
using CC01_0001.Services;
using CC01_0001.Data;
using Microsoft.AspNetCore.Mvc;
using CC01_0001.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CC01_0001.Controllers;

public class BinanceDataController : Controller
{
    private readonly ApplicationDbContext _context;

    public BinanceDataController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var trades = await _context.BinanceTrades
            .OrderByDescending(t => t.TradeTime)
            .Take(20)
            .ToListAsync();

        return View(trades);

    }
}

