/*
    ViewModels/CryptoViewModel.cs
    Version: 1.1.0
    (c) 2024, Minh Tri Tran, with assistance from Google's Gemini - Licensed under CC BY 4.0
    https://creativecommons.org/licenses/by/4.0/
*/
using System.Collections.Generic;

namespace CC01_0001.ViewModels
{
    public class CryptoViewModel
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Change24h { get; set; }
        public decimal MarketCap { get; set; }
        public decimal Volume24h { get; set; }
        public string Sentiment { get; set; }
        public string Region { get; set; }
        public string Category { get; set; }
    }

    public class CryptoMarketViewModel
    {
        public List<CryptoViewModel> CryptoData { get; set; }
    }


    // Example usage:
    // var viewModel = SampleDataGenerator.GenerateSampleData(200); // Generates 200 sample data points

    public class SampleDataGenerator
    {
        private static readonly string[] Symbols = { "BTC", "ETH", "ADA", "SOL", "DOGE", "AVAX", "LINK", "DOT", "MANA", "SAND", "XRP", "LTC", "BCH", "XLM", "EOS", "TRX", "ETC", "BNB", "USDT", "USDC", "SHIB", "MATIC", "DAI", "UNI", "AAVE", "COMP", "MKR", "SNX", "YFI", "CRV", "SUSHI", "1INCH", "BAL", "ZRX", "BAT", "OMG", "REN", "ENJ", "CHZ", "THETA", "EGLD", "FIL", "HNT", "ICP", "IOTA", "KSM", "NEAR", "ONE", "QTUM", "RVN", "SC", "XMR", "ZEC", "DASH", "GRT", "CELO", "ALGO", "ATOM", "VET", "XEM" };
        private static readonly string[] Categories = { "Currency", "Platform", "DeFi", "NFT" };
        private static readonly string[] Sentiments = { "bullish", "bearish", "neutral" };
        private static readonly string[] Regions = { "North America", "Europe", "Asia", "Global", "Africa", "South America", "Oceania" };

        private static readonly Random Random = new Random();

        public static CryptoMarketViewModel GenerateSampleData(int count)
        {
            var cryptoData = new List<CryptoViewModel>();
            for (int i = 0; i < count; i++)
            {
                string symbol = Symbols[Random.Next(Symbols.Length)];
                string name = $"Crypto {i + 1} ({symbol})"; // Simplified name generation
                decimal price = (decimal)(Random.NextDouble() * 500 + 0.01); // Price from 0.01 to 500
                decimal change24h = (decimal)(Random.NextDouble() * 10 - 5); // Change from -5 to +5
                decimal marketCap = (decimal)(Random.NextDouble() * 100000000000); // Up to 100 Billion
                decimal volume24h = (decimal)(Random.NextDouble() * 10000000000); // Up to 10 Billion
                string sentiment = Sentiments[Random.Next(Sentiments.Length)];
                string region = Regions[Random.Next(Regions.Length)];
                string category = Categories[Random.Next(Categories.Length)];

                cryptoData.Add(new CryptoViewModel
                {
                    Symbol = symbol,
                    Name = name,
                    Price = price,
                    Change24h = change24h,
                    MarketCap = marketCap,
                    Volume24h = volume24h,
                    Sentiment = sentiment,
                    Region = region,
                    Category = category
                });
            }

            return new CryptoMarketViewModel { CryptoData = cryptoData };
        }
    }
}