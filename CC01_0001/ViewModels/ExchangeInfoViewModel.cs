// ViewModels/ExchangeInfoViewModel.cs
// Version: 1.1
// (c) 2024, Minh Tri Tran - Licensed under CC BY 4.0
// https://creativecommons.org/licenses/by/4.0/
namespace CC01_0001.ViewModels;

public class ExchangeInfoViewModel
{
    public string Timezone { get; set; }
    public long ServerTime { get; set; }
    public List<SymbolViewModel> Symbols { get; set; }
}

public class SymbolViewModel
{
    public string Symbol { get; set; }
    public string Status { get; set; }
    public string BaseAsset { get; set; }
    public string QuoteAsset { get; set; }
    public string MinPrice { get; set; }
    public string MaxPrice { get; set; }
}