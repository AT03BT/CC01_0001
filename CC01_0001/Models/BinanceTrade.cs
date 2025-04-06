/*
    Controllers/HomeController.cs
    Version: 1.0.0
    (c) 2024, Minh Tri Tran, with assistance from Google's Gemini - Licensed under CC BY 4.0
    https://creativecommons.org/licenses/by/4.0/
*/
using System.Text.Json.Serialization;

namespace CC01_0001.Models;

public class BinanceTrade
{
    public int Id { get; set; } // Add an ID for database primary key
    [JsonPropertyName("e")]
    public string EventType { get; set; }

    [JsonPropertyName("E")]
    public long EventTime { get; set; }

    [JsonPropertyName("s")]
    public string Symbol { get; set; }

    [JsonPropertyName("t")]
    public long TradeId { get; set; }

    [JsonPropertyName("p")]
    public string Price { get; set; }

    [JsonPropertyName("q")]
    public string Quantity { get; set; }

    [JsonPropertyName("T")]
    public long TradeTime { get; set; }

    [JsonPropertyName("m")]
    public bool IsBuyerMarketMaker { get; set; }

    [JsonPropertyName("M")]
    public bool Ignore { get; set; }
}