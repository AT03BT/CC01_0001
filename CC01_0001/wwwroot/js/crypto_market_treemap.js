// crypto_market_treemap.js
// Version: 2.5
// (c) 2024, Minh Tri Tran - Licensed under CC BY 4.0
// https://creativecommons.org/licenses/by/4.0/

document.addEventListener("DOMContentLoaded", () => {

    try {
        // 1. Get filter elements
        const currencyFilter = document.getElementById("currencyFilter");
        const platformFilter = document.getElementById("platformFilter");
        const defiFilter = document.getElementById("defiFilter");
        const nftFilter = document.getElementById("nftFilter");

        const bullishFilter = document.getElementById("bullishFilter");
        const bearishFilter = document.getElementById("bearishFilter");
        const neutralFilter = document.getElementById("neutralFilter");


        // Get the width and height of the crypto-map div
        const container = document.getElementById("crypto-map");
        const width = container.clientWidth;
        const height = container.clientHeight;

        // Create a hierarchical structure based on 'category'
        function updateChart() {

            const filteredData = cryptoData.filter(d =>
                (currencyFilter.checked && d.category === "Currency") ||
                (platformFilter.checked && d.category === "Platform") ||
                (defiFilter.checked && d.category === "DeFi") ||
                (nftFilter.checked && d.category === "NFT")
            ).filter(d =>
                (bullishFilter.checked && d.sentiment === "bullish") ||
                (bearishFilter.checked && d.sentiment === "bearish") ||
                (neutralFilter.checked && d.sentiment === "neutral")
            );

            const groupedData = d3.group(filteredData, d => d.category);

            const root = d3.hierarchy({
                key: "Crypto Market", // A root name
                values: Array.from(groupedData, ([key, values]) => ({
                    key: key,              // Category name
                    values: values         // Array of crypto data objects
                })),
            }, (d) => d.values) //This is the children accessor
                .sum(d => d.marketCap)
                .sort((a, b) => b.value - a.value);

            // Create the treemap layout
            const treemap = d3.treemap()
                .size([width, height])
                .padding(0) // Set padding to 0 for a cleaner look
                (root); // Pass the root to the treemap function

            // Create the SVG container
            d3.select("#crypto-map").select("svg").remove();
            const svg = d3.select("#crypto-map")
                .append("svg")
                .attr("width", "100%") // Make the SVG take up 100% of its container's width
                .attr("height", "100%")// Make the SVG take up 100% of its container's height
                .attr("viewBox", [0, 0, width, height]) // Define the coordinate system for the SVG
                .attr("preserveAspectRatio", "xMidYMid meet"); // Ensure the treemap scales proportionally

            // Create the tooltip
            const tooltip = d3.select("body")
                .append("div")
                .attr("class", "tooltip")
                .style("opacity", 0)
                .style("position", "absolute")
                .style("background-color", "white")
                .style("border", "solid")
                .style("border-width", "1px")
                .style("border-radius", "5px")
                .style("padding", "5px");

            // Mouseover event handler
            const mouseover = function (event, d) {
                tooltip
                    .style("opacity", 1);
                d3.select(this)
                    .style("stroke", "black")
                    .style("opacity", 1);
            };

            // Mousemove event handler
            const mousemove = function (event, d) {
                tooltip
                    .html(`Symbol: ${d.data.symbol}<br>Name: ${d.data.name}<br>Market Cap: $${d.data.marketCap}`)
                    .style("left", (event.pageX + 10) + "px")
                    .style("top", (event.pageY - 28) + "px");
            };

            // Mouseleave event handler
            const mouseleave = function (event, d) {
                tooltip
                    .style("opacity", 0);
                d3.select(this)
                    .style("stroke", "black")
                    .style("opacity", 0.8);
            };


            // Create the rectangles for the treemap
            svg.selectAll("rect")
                .data(root.descendants().filter(d => d.depth > 0)) //The filter prevents the root from being displayed
                .enter()
                .append("rect")
                .attr("x", function (d) { return d.x0; })
                .attr("y", function (d) { return d.y0; })
                .attr("width", function (d) { return d.x1 - d.x0; })
                .attr("height", function (d) { return d.y1 - d.y0; })
                .style("stroke", "black")
                .style("fill", d => {
                    // Use the 'sentiment' property to define colors
                    if (d.data.sentiment === "bullish") {
                        return "green";
                    } else if (d.data.sentiment === "bearish") {
                        return "red";
                    } else {
                        return "gray"; // Default color
                    }
                })
                .on("mouseover", mouseover)
                .on("mousemove", mousemove)
                .on("mouseleave", mouseleave);

            // Add text labels to the rectangles
            svg.selectAll("text")
                .data(root.descendants().filter(d => d.depth > 0 && d.height === 0)) // only leaf nodes
                .enter()
                .append("text")
                .attr("x", function (d) { return d.x0 + 5 })    // +10 to adjust position (horizontal)
                .attr("y", function (d) { return d.y0 + 20 })    // +20 to adjust position (vertical)
                .text(function (d) { return d.data.symbol })
                .attr("font-size", "11px")
                .attr("fill", "white");
        }

        const filters = ["currencyFilter", "platformFilter", "defiFilter", "nftFilter", "bullishFilter", "bearishFilter", "neutralFilter"];
        filters.forEach(filterId => {
            const filterElement = document.getElementById(filterId);
            if (filterElement) {  // Check if the element exists
                filterElement.addEventListener("change", updateChart);
            } else {
                console.warn(`Filter element with ID '${filterId}' not found.`);
            }
        });
        updateChart();

    } catch (error) {
        console.error("Error initializing or drawing the chart:", error);
        const mapDiv = document.getElementById("crypto-map");
        if (mapDiv) {
            mapDiv.innerHTML = "<p style='color:red;'>Error loading the crypto market map. Please check the console for details.</p>";
        }
    }
    // Add event listener to filters after the DOM loads
    console.log("D3 script loaded");  // Helpful message
});