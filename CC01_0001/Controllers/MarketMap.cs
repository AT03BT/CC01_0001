using Microsoft.AspNetCore.Mvc;
using CC01_0001.ViewModels;
using System.Collections.Generic;

namespace CC01_0001.Controllers
{
    // Controllers/CryptoController.cs
    // Version: 1.1
    // (c) 2024, Minh Tri Tran - Licensed under CC BY 4.0
    // https://creativecommons.org/licenses/by/4.0/
    public class MarketMap : Controller
    {
        public IActionResult Index()
        {
            var viewModel = SampleDataGenerator.GenerateSampleData(200); // Generate 200 data points
            return View(viewModel);
        }

        public IActionResult MarketMap1()
        {
            var viewModel = SampleDataGenerator.GenerateSampleData(200); // Generate 200 data points
            return View(viewModel);
        }

        public IActionResult MarketMap1_2()
        {
            var viewModel = SampleDataGenerator.GenerateSampleData(200); // Generate 200 data points
            return View(viewModel);
        }

        public IActionResult MarketMap1_3()
        {
            var viewModel = SampleDataGenerator.GenerateSampleData(200); // Generate 200 data points
            return View(viewModel);
        }

        public IActionResult MarketMap2()
        {
            var viewModel = SampleDataGenerator.GenerateSampleData(200); // Generate 200 data points
            return View(viewModel);
        }

        public IActionResult MarketMap3()
        {
            var viewModel = SampleDataGenerator.GenerateSampleData(200); // Generate 200 data points
            return View(viewModel);
        }

        public IActionResult MarketMap4()
        {
            var viewModel = SampleDataGenerator.GenerateSampleData(200); // Generate 200 data points
            return View(viewModel);
        }
    }

}