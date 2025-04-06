using Microsoft.AspNetCore.Mvc;
using CC01_0001.ViewModels;
using System.Collections.Generic;
using System.Linq;
using CC01_0001.Data; // Add this
using Microsoft.EntityFrameworkCore; // Add this
using Microsoft.Extensions.Logging;
using CC01_0001.Data;
using CC01_0001.ViewModels;

namespace BinanceInfoApp.Controllers;

public class ExchangeInfoController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ExchangeInfoController> _logger;

    public ExchangeInfoController(
        ApplicationDbContext context,
        ILogger<ExchangeInfoController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        //try
        //{
        //    var symbols = await _context.BinanceExchangeInfos.Sym

        //    var viewModel = new ExchangeInfoViewModel
        //    {
        //        // You might need to fetch timezone and serverTime separately if you store them
        //        // or have a default value if they are not stored.
        //        Timezone = "UTC",
        //        ServerTime = DateTime.UtcNow.Ticks,
        //        Symbols = symbols.Select(s => new SymbolViewModel
        //        {
        //            Symbol = s.symbol,
        //            Status = s.status,
        //            BaseAsset = s.baseAsset,
        //            QuoteAsset = s.quoteAsset,
        //            MinPrice = s.minPrice.ToString(),
        //            MaxPrice = s.maxPrice.ToString()
        //        }).ToList()
        //    };

        //    return View(viewModel);
        //}
        //catch (Exception ex)
        //{
        //    _logger.LogError(ex, "Error retrieving exchange info from database.");
        //    return View(new ExchangeInfoViewModel { Symbols = new List<SymbolViewModel>() }); // Return empty view
        //}
        return View();
    }
}