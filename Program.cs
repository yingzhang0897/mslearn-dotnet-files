using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

class Program
{
    // Entry point of the application
    public static void Main()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var storesDir = Path.Combine(currentDirectory, "stores");

        var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
        Directory.CreateDirectory(salesTotalDir);

        var storeDir201 = Path.Combine(storesDir, "201");
        var storeDir202 = Path.Combine(storesDir, "202");
        var storeDir203 = Path.Combine(storesDir, "203");
        var storeDir204 = Path.Combine(storesDir, "204");

        var salesFiles = FindFiles(storesDir);
        var salesFiles201 = FindFiles(storeDir201);
        var salesFiles202 = FindFiles(storeDir202);
        var salesFiles203 = FindFiles(storeDir203);
        var salesFiles204 = FindFiles(storeDir204);

        var salesTotal = CalculateSalesTotal(salesFiles);
        var salesStore201 = CalculateSalesTotal(salesFiles201);
        var salesStore202 = CalculateSalesTotal(salesFiles202);
        var salesStore203 = CalculateSalesTotal(salesFiles203);
        var salesStore204 = CalculateSalesTotal(salesFiles204);

        File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

        List<Store> stores = new List<Store>
        {
            new Store { Name = "201", Sales = salesStore201 },
            new Store { Name = "202", Sales = salesStore202 },
            new Store { Name = "203", Sales = salesStore203 },
            new Store { Name = "204", Sales = salesStore204 }
        };

        StringBuilder report = new StringBuilder();
        report.AppendLine("Sales Summary");
        report.AppendLine("=====================");
        report.AppendLine();

        double totalSales = 0;
        foreach (var store in stores)
        {
            report.AppendLine($"Store: {store.Name}");
            report.AppendLine($"Sales: {store.Sales:F2}");  // Format sales with 2 decimal places
            totalSales += store.Sales;
        }

        report.AppendLine();
        report.AppendLine($"Total Stores: {stores.Count}");
        report.AppendLine($"Total Sales: {totalSales:F2}");  // Format total sales with 2 decimal places

        string reportContent = report.ToString();
        File.WriteAllText(Path.Combine(salesTotalDir, "summary-report.txt"), reportContent);

        Console.WriteLine($"New files are created under {salesTotalDir}");
    }

    static IEnumerable<string> FindFiles(string folderName)
    {
        List<string> salesFiles = new List<string>();
        var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

        foreach (var file in foundFiles)
        {
            var extension = Path.GetExtension(file);
            if (extension == ".json")
            {
                salesFiles.Add(file);
            }
        }
        return salesFiles;
    }

    static double CalculateSalesTotal(IEnumerable<string> salesFiles)
    {
        double salesTotal = 0;

        foreach (var file in salesFiles)
        {
            string salesJson = File.ReadAllText(file);
            SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
            salesTotal += data?.Total ?? 0;
        }

        return salesTotal;
    }
}

record SalesData(double Total);

public class Store
{
    public string Name { get; set; }
    public double Sales { get; set; }
}
