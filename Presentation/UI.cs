
using inventory.Models;
using inventory.Services;
using System;
using System.Collections.Generic;

namespace inventory.Presentation
{
    public class UI
    {
        public static void ShowMessage(string message, string messageType)
        {
            switch (messageType)
            {
                case "error":
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                case "success":
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;

                case "info":
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;

                case "warning":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                case "3orless":
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;

                case "6orless":
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;

                case "statistics":
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    break;

                default:
                    Console.ResetColor();
                    break;
            }

            Console.WriteLine(message);
            Console.ResetColor();
            Console.WriteLine();
        }

        // This display is not currently in use.
        public static void ShowProduct(ProductManager productManager)
        {
            DateTime today = DateTime.Today;

            ShowMessage(
                "ASSET TRACKING - DEVICES BY OFFICE",
                "info");

            foreach (Category category in Enum.GetValues(typeof(Category)))
            {
                Console.WriteLine($"{(int)category}. {category}");
            }
            foreach (Category category in Enum.GetValues(typeof(Category)))
            {
                ShowMessage(
                    $"{category.ToString().ToUpper()}",
                    "statistics");

                //List<Product> products = productManager.GetProductsByCategory(category);
                List<Product> products = productManager._products.FindAll(p => p.CategoryId == category);
                if (products.Count == 0)
                {
                    ShowMessage("No products found.", "info");
                    continue;
                }

                foreach (Product product in products)
                {


                    string productInfo =
                        $"ID: {product.Id} | " +
                        $"{product.Name}\n" +
                        $"Category: {Models.Category.GetName(product.CategoryId)}\n" +
                        $"Quantity: {product.Quantity} {Models.Unit.GetName(product.UnitId)}" +
                        $"Price per unit: {product.PricePerUnit:N2} " +
                        $"Total Price: {product.TotalPrice:N2}\n" +
                        $"Added on date: {product.AddedOn:yyyy-MM-dd}\n" +
                        $"Last Modified on: {product.LastModifiedOn:yyyy-MM-dd}";



                    ShowMessage(productInfo, "info");
                }
            }
        }

        // The display in use is a more compact table format for showing devices by office.
        /*public static void ShowDevicesByOffice(DeviceManager deviceManager)
        {
            DateTime today = DateTime.Today;

            foreach (Office office in deviceManager.GetOffices())
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"OFFICE: {office.Country} ({office.Currency})");
                Console.ResetColor();

                // Table header
                Console.WriteLine(new string('-', 135));

                Console.WriteLine(
                    $"{"ID",-5} {"Type",-12} {"Brand",-12} {"Model",-22} " +
                    $"{"Purchased",-16} {"Use before",-16}" +
                    $"{"Price USD",-17} {"Local price("+office.Currency+")",-18}" +
                    $"{"Status", - 9}");

                Console.WriteLine(new string('-', 135));

                List<Device> devices = deviceManager.GetDevicesByOffice(office.Id);

                if (devices.Count == 0)
                {
                    Console.WriteLine("No devices registered for this office.");
                }

                foreach (Device device in devices)
                {
                    DeviceLifeStatus status = deviceManager.GetDeviceLifeStatus(device, today);

                    Console.ForegroundColor = status switch
                    {
                        DeviceLifeStatus.Red => ConsoleColor.Red,
                        DeviceLifeStatus.Yellow => ConsoleColor.Yellow,
                        _ => ConsoleColor.White
                    };

                    string mainPrice = $"{device.PriceMainOffice:N2}";

                    string localPrice = $"{device.PriceLocalOffice:N2}";

                    string model = Fit(device.Model, 22);
                    string brand = Fit(device.Brand, 12);

                    string statusText = device.EndOfLifeDate < today ? " EXPIRED" : "";

                    Console.WriteLine(
                        $"{device.Id,-5} " +
                        $"{device.DeviceType,-12} " +
                        $"{brand,-12} " +
                        $"{model,-22} " +
                        $"{device.PurchaseDate:yyyy-MM-dd}\t" + 
                        $"{device.EndOfLifeDate:yyyy-MM-dd}\t" +
                        $"{mainPrice,-17} " +
                        $"{localPrice,-17}" +
                        statusText);

                    Console.ResetColor();
                }

                Console.WriteLine(new string('-', 135));
            }
        }

        private static string Fit(string value, int width)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if (value.Length > width)
                return value.Substring(0, width - 3) + "...";

            return value;
        }*/
    }
}

