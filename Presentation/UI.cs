
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

        //This method handles the user input when adding a new product.
        public static Product? GetProductInput(int nextId)
        {
            string? name = ReadRequiredText("Product name");
            if (name == null)
                return null;

            int? categoryId = ReadCategoryId();
            if (categoryId == null)
                return null;

            decimal? price = ReadDecimal("Price per unit");
            if (price == null)
                return null;

            decimal? quantity = ReadDecimal("Quantity");
            if (quantity == null)
                return null;

            int? unitId = ReadUnitId();
            if (unitId == null)
                return null;

            return new Product(nextId, name, price.Value, categoryId.Value, quantity.Value, unitId.Value);
        }

        private static string? ReadRequiredText(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt} (q to cancel): ");
                string? input = Console.ReadLine();

                if (UserCancel(input))
                    return null;

                if (string.IsNullOrWhiteSpace(input))
                {
                    UI.ShowMessage($"{prompt} cannot be empty.", "error");
                    continue;
                }

                return input.Trim();
            }
        }
        private static decimal? ReadDecimal(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt} (q to cancel): ");
                string? input = Console.ReadLine();

                if (UserCancel(input))
                    return null;

                if (!decimal.TryParse(input, out decimal inputValue) || inputValue < 0)
                {
                    UI.ShowMessage($"Invalid {prompt}. Please try again.", "error");
                    continue;
                }

                return inputValue;
            }
        }
        private static int? ReadCategoryId()
        {
            while (true)
            {
                Console.WriteLine("Select a category:");
                foreach (Category category in Enum.GetValues(typeof(Category)))
                {
                    Console.WriteLine($"{(int)category}. {category}");
                }
                Console.Write("Enter the category number (q to cancel): ");
                string? input = Console.ReadLine();
                if (UserCancel(input))
                    return null;
                if (!int.TryParse(input, out int categoryId) || !Enum.IsDefined(typeof(Category), categoryId))
                {
                    UI.ShowMessage("Invalid category. Please try again.", "error");
                    continue;
                }
                return categoryId;
            }
        }
        private static int? ReadUnitId()
        {
            while (true)
            {
                Console.WriteLine("Select a unit:");
                foreach (Unit unit in Enum.GetValues(typeof(Unit)))
                {
                    Console.WriteLine($"{(int)unit}. {unit}");
                }
                Console.Write("Enter the unit number (q to cancel): ");
                string? input = Console.ReadLine();
                if (UserCancel(input))
                    return null;

                if (!int.TryParse(input, out int unitId) || !Enum.IsDefined(typeof(Unit), unitId))
                {
                    UI.ShowMessage("Invalid unit. Please try again.", "error");
                    continue;
                }
                return unitId;
            }
        }

        // Repetitive check for exit - separated in own method.
        private static bool UserCancel(string input)
        {
            return input == null || input.Trim().Equals("q", StringComparison.OrdinalIgnoreCase);
        }
        // This display is not currently in use.
        public static void ShowProductByCategory(ProductManager productManager)
        {
            List<Product> products = productManager.GetProducts();
            DateTime today = DateTime.Today;

            ShowMessage(
                "All products by Category",
                "info");

            /*foreach (Category category in Enum.GetValues(typeof(Category)))
            {
                Console.WriteLine($"{(int)category}. {category}");
            }*/
            foreach (Category category in Enum.GetValues(typeof(Category)))
            {
                ShowMessage(
                    $"{category.ToString().ToUpper()}",
                    "statistics");

                //List<Product> products = productManager.GetProductsByCategory(category);
                List<Product> displayProducts = products.FindAll(p => p.CategoryId == category);
                if (displayProducts.Count == 0)
                {
                    ShowMessage("No products found.", "info");
                    continue;
                }

                foreach (Product product in displayProducts)
                {


                    string productInfo =
                        $"ID: {product.Id} | " +
                        $"{product.Name}\n" +
                        $"Category: {Models.Category.GetName(product.CategoryId)}\n" +
                        $"Quantity: {product.Quantity} {Models.Unit.GetName(product.UnitId)}\n" +
                        $"Price per unit: {product.PricePerUnit:N2}\n" +
                        $"Total Price: {product.TotalPrice:N2}\n" +
                        $"Added on date: {product.AddedOn:yyyy-MM-dd}\n" +
                        $"Last Modified on: {product.LastModifiedOn:yyyy-MM-dd}\n";



                    ShowMessage(productInfo, "info");
                }
            }
        }

        // Propt the user to add a new product to the inventory.
        public static void AddProduct(ProductManager productManager)
        {
            Product? newProduct = GetProductInput(productManager.NextId);

            if (newProduct == null)
            {
                ShowMessage("Adding the product was cancelled.", "info");
                return;
            }

            productManager.AddProduct(newProduct);
            productManager.SaveToFile();
            ShowMessage($"Product '{newProduct.Name}' added successfully!", "success");
        }

        // The display in use is a more compact table format for showing devices by office.
        public static void ShowProducts(ProductManager productManager)
        {
            DateTime today = DateTime.Today;

            foreach (Category category in Enum.GetValues(typeof(Category)))
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Category: {category.ToString().ToUpper()}");
                Console.ResetColor();

                // Table header
                Console.WriteLine(new string('-', 135));

                Console.WriteLine(
                    $"{"ID",-5} {"Name",-20} {"Category",-15} {"Quantity",-10} " +
                    $"{"Price per unit",-16} {"Total Price",-16}" +
                    $"{"Added on date",-16} {"Last Modified on",-16}");


                Console.WriteLine(new string('-', 135));

                List<Product> products = productManager.GetProductsByCategory(category);

                if (products.Count == 0)
                {
                    Console.WriteLine("No products found for this category.");
                }

                foreach (Product product in products)
                {
                    Console.WriteLine(
                        $"{product.Id,-5} " +
                        $"{product.Name,-20} " +
                        $"{Models.Category.GetName(product.CategoryId),-15} " +
                        $"{product.Quantity,-10} " +
                        $"{product.PricePerUnit,-16} " +
                        $"{product.TotalPrice,-16} " +
                        $"{product.AddedOn:yyyy-MM-dd,-16} " +
                        $"{product.LastModifiedOn:yyyy-MM-dd,-16}");
                }
            }
        }
    }
}           

              /*          _ => ConsoleColor.White
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
