// crypto_market_treemap.js
// Version: 1.9
// (c) 2024, Minh Tri Tran - Licensed under CC BY 4.0
// https://creativecommons.org/licenses/by/4.0/

document.addEventListener("DOMContentLoaded", () => {
    const cryptoData = [
        { symbol: "BTC", name: "Bitcoin", price: 62500, change24h: 2.5, marketCap: 1200000000000, volume24h: 35000000000, sentiment: "bullish", region: "North America", category: "Currency" },
        { symbol: "ETH", name: "Ethereum", price: 4500, change24h: -1.0, marketCap: 550000000000, volume24h: 20000000000, sentiment: "neutral", region: "Europe", category: "Platform" },
        { symbol: "ADA", name: "Cardano", price: 1.20, change24h: 0.8, marketCap: 40000000000, volume24h: 2000000000, sentiment: "bullish", region: "Asia", category: "Platform" },
        { symbol: "SOL", name: "Solana", price: 180, change24h: 3.2, marketCap: 80000000000, volume24h: 5000000000, sentiment: "bullish", region: "North America", category: "Platform" },
        { symbol: "DOGE", name: "Dogecoin", price: 0.15, change24h: -2.0, marketCap: 20000000000, volume24h: 1500000000, sentiment: "bearish", region: "Global", category: "Currency" },
        { symbol: "AVAX", name: "Avalanche", price: 95, change24h: 1.5, marketCap: 25000000000, volume24h: 1000000000, sentiment: "bullish", region: "Europe", category: "Platform" },
        { symbol: "LINK", name: "Chainlink", price: 18, change24h: -0.5, marketCap: 9000000000, volume24h: 800000000, sentiment: "neutral", region: "Global", category: "DeFi" },
        { symbol: "DOT", name: "Polkadot", price: 22, change24h: 0.2, marketCap: 24000000000, volume24h: 900000000, sentiment: "neutral", region: "Asia", category: "Platform" },
        { symbol: "MANA", name: "Decentraland", price: 3.0, change24h: 4.0, marketCap: 5500000000, volume24h: 600000000, sentiment: "bullish", region: "Global", category: "NFT" },
        { symbol: "SAND", name: "Sandbox", price: 4.5, change24h: -1.2, marketCap: 6000000000, volume24h: 700000000, sentiment: "bearish", region: "Global", category: "NFT" }
    ];

    const width = 800;
    const height = 600;

    const svg = d3.select("#crypto-map")
        .append("svg")
        .attr("width", "100%")
        .attr("height", "100%")
        .attr("viewBox", [0, 0, width, height])
        .attr("preserveAspectRatio", "xMidYMid meet");

    const tooltip = d3.select("body")
        .append("div")
        .attr("class", "tooltip")
        .style("position", "absolute")
        .style("background-color", "white")
        .style("border", "1px solid black")
        .style("padding", "5px")
        .style("display", "none");

    function updateChart() {
        const currencyFilter = document.getElementById("currencyFilter").checked;
        const platformFilter = document.getElementById("platformFilter").checked;
        const defiFilter = document.getElementById("defiFilter").checked;
        const nftFilter = document.getElementById("nftFilter").checked;

        const bullishFilter = document.getElementById("bullishFilter").checked;
        const bearishFilter = document.getElementById("bearishFilter").checked;
        const neutralFilter = document.getElementById("neutralFilter").checked;

        const filteredData = cryptoData.filter(d =>
            (currencyFilter && d.category === "Currency") ||
            (platformFilter && d.category === "Platform") ||
            (defiFilter && d.category === "DeFi") ||
            (nftFilter && d.category === "NFT")
        ).filter(d =>
            (bullishFilter && d.sentiment === "bullish") ||
            (bearishFilter && d.sentiment === "bearish") ||
            (neutralFilter && d.sentiment === "neutral")
        );

        const root = d3.hierarchy({ children: filteredData })
            .sum(d => d.marketCap)
            .sort((a, b) => b.value - a.value);

        const treemapLayout = d3.treemap()
            .size([width, height])
            .padding(1);

        treemapLayout(root);

        const nodes = svg.selectAll("rect")
            .data(root.leaves(), d => d.data.symbol); // Key function for transitions

        // Exit
        nodes.exit()
            .transition()
            .duration(300)
            .style("opacity", 0)
            .remove();

        // Enter
        const enterNodes = nodes.enter()
            .append("rect")
            .attr("x", d => d.x0)
            .attr("y", d => d.y0)
            .attr("width", d => d.x1 - d.x0)
            .attr("height", d => d.y1 - d.y0)
            .style("fill", d => {
                if (d.data.sentiment === "bullish") return "green";
                if (d.data.sentiment === "bearish") return "red";
                return "gray";
            })
            .style("opacity", 0) // Fade in
            .on("mouseover", (event, d) => {
                tooltip.transition()
                    .duration(200)
                    .style("opacity", 0.9)
                    .style("display", "block");

                tooltip.html(`<b>${d.data.symbol}</b><br/>Market Cap: $${(d.data.marketCap / 1000000000).toFixed(2)} Billion`)
                    .style("left", (event.pageX + 10) + "px")
                    .style("top", (event.pageY - 28) + "px");
            })
            .on("mousemove", (event) => {
                tooltip
                    .style("left", (event.pageX + 10) + "px")
                    .style("top", (event.pageY - 28) + "px");
            })
            .on("mouseout", () => {
                tooltip.transition()
                    .duration(500)
                    .style("opacity", 0)
                    .style("display", "none");
            });

        // Enter + Update
        enterNodes.merge(nodes)
            .transition()
            .duration(300)
            .attr("x", d => d.x0)
            .attr("y", d => d.y0)
            .attr("width", d => d.x1 - d.x0)
            .attr("height", d => d.y1 - d.y0)
            .style("fill", d => {
                if (d.data.sentiment === "bullish") return "green";
                if (d.data.sentiment === "bearish") return "red";
                return "gray";
            })
            .style("opacity", 1); // Fade in

        // Add text labels (simplified)
        svg.selectAll("text").remove();  // Clear existing labels

        svg.selectAll("text")
            .data(root.leaves())
            .enter()
            .append("text")
            .attr("x", function (d) { return d.x0 + 5; })    // +10 to adjust position (horizontal)
            .attr("y", function (d) { return d.y0 + 20; })    // +20 to adjust position (vertical)
            .text(function (d) { return d.data.symbol; })
            .attr("font-size", "11px")
            .attr("fill", "white");
    }

    // Attach Event listeners to filters
    const filters = ["currencyFilter", "platformFilter", "defiFilter", "nftFilter", "bullishFilter", "bearishFilter", "neutralFilter"];
    filters.forEach(filterId => {
        const filterElement = document.getElementById(filterId);
        if (filterElement) {
            filterElement.addEventListener("change", updateChart);
        } else {
            console.warn(`Filter element with ID '${filterId}' not found.`);
        }
    });

    // Initial chart draw
    updateChart();

    console.log("D3 script loaded");  // Helpful message
});