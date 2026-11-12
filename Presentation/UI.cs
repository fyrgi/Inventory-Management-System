
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

        private static void WriteTableHeader(params (string Name, int Width)[] columns)
        {
            int tableWidth = columns.Sum(column => column.Width) + columns.Length - 1;

            for (int i = 0; i < columns.Length; i++)
            {
                if (i > 0)
                    Console.Write(" ");

                (string name, int width) = columns[i];
                Console.Write(name.PadRight(width));
            }

            Console.WriteLine();
            Console.WriteLine(new string('-', tableWidth));
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
        
        private static int? ReadEnumId<TEnum>(string enumName)
            where TEnum : struct, Enum
        {
            while (true)
            {
                Console.WriteLine($"Select a {enumName}:");
                foreach (TEnum enumValue in Enum.GetValues<TEnum>())
                {
                    Console.WriteLine($"{Convert.ToInt32(enumValue)}. {enumValue}");
                }

                Console.Write($"Enter the {enumName} number (q to cancel): ");
                string? input = Console.ReadLine();

                if (UserCancel(input))
                    return null;

                if (!int.TryParse(input, out int enumId) || !Enum.IsDefined(typeof(TEnum), enumId))
                {
                    UI.ShowMessage($"Invalid {enumName}. Please try again.", "error");
                    continue;
                }

                return enumId;
            }
        }

        private static int? ReadCategoryId()
        {
            return ReadEnumId<Category>("category");
        }

        private static int? ReadUnitId()
        {
            return ReadEnumId<Unit>("unit");
        }

        // Repetitive check for exit - separated in own method.
        private static bool UserCancel(string? input)
        {
            return input == null || input.Trim().Equals("q", StringComparison.OrdinalIgnoreCase);
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
            ShowMessage($"Product '{newProduct.Name}' added successfully!", "success");
        }

        public static void SearchProducts(ProductManager productManager)
        {
            Console.Write("Enter a product ID, name, or category name to search: ");
            string searchTerm = Console.ReadLine()?.Trim() ?? string.Empty;

            List<Product> matchingProducts = productManager.Search(searchTerm);

            if (matchingProducts.Count == 0)
            {
                ShowMessage($"No products match your search {searchTerm}.", "info");
            }
            else
            {
                ShowMessage($"Search results for {searchTerm}", "info");
                WriteTableHeader(
                    ("ID", 5),
                    ("Name", 25),
                    ("Category", 20),
                    ("Quantity", 15),
                    ("PPU", 15),
                    ("Total price", 15),
                    ("Added On", 20),
                    ("Last Modified On", 20));

                foreach (Product product in matchingProducts)
                {
                    WriteHighlightedValue(product.Id.ToString().PadRight(5), searchTerm);
                    Console.Write(" ");
                    WriteHighlightedValue(product.Name.PadRight(25), searchTerm);
                    Console.Write(" ");
                    WriteHighlightedValue(product.CategoryId.ToString().PadRight(20), searchTerm);
                    Console.WriteLine(
                        " {0,-15:N2} {1,-15:N2} {2,-15:N2} {3,-20:yyyy-MM-dd} {4,-20:yyyy-MM-dd}",
                        product.Quantity,
                        product.PricePerUnit,
                        product.TotalPrice,
                        product.AddedOn,
                        product.LastModifiedOn);
                }
            }
            Console.WriteLine();
        }

        private static void WriteHighlightedValue(string value, string searchTerm)
        {
            int currentIndex = 0;

            while (currentIndex < value.Length)
            {
                int matchIndex = value.IndexOf(searchTerm, currentIndex, StringComparison.OrdinalIgnoreCase);
                if (matchIndex < 0)
                {
                    Console.Write(value[currentIndex..]);
                    return;
                }

                Console.Write(value[currentIndex..matchIndex]);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write(value.Substring(matchIndex, searchTerm.Length));
                Console.ResetColor();
                currentIndex = matchIndex + searchTerm.Length;
            }
        }

        public static void EditProduct(ProductManager productManager)
        {
            while (true)
            {
                Console.Write("Enter the product ID to edit, 'v' to view products, or 'q' to cancel: ");
                string? input = Console.ReadLine();

                if (UserCancel(input))
                {
                    ShowMessage("Product editing was cancelled. Returning to the main menu.", "info");
                    return;
                }

                if (input!.Trim().Equals("v", StringComparison.OrdinalIgnoreCase))
                {
                    ShowProductsForSelection(productManager);
                    continue;
                }

                if (!int.TryParse(input, out int id))
                {
                    ShowMessage("Invalid product ID. Enter a whole number, 'v', or 'q'.", "error");
                    continue;
                }

                Product? product = productManager.GetProductById(id);
                if (product == null)
                {
                    ShowMessage($"No product with ID {id} was found.", "warning");
                    continue;
                }

                EditSelectedProduct(productManager, product);
                return;
            }
        }

        private static void EditSelectedProduct(ProductManager productManager, Product product)
        {
            string editedName = product.Name;
            Category editedCategory = product.CategoryId;
            Unit editedUnit = product.UnitId;
            decimal editedPrice = product.PricePerUnit;
            decimal editedQuantity = product.Quantity;

            while (true)
            {
                ShowMessage($"Editing product {product.Id} - {product.Name}", "info");
                Console.WriteLine("What do you want to change?");
                WriteEditOption(1, "Change name", product.Name, editedName);
                WriteEditOption(2, "Change category", product.CategoryId.ToString(), editedCategory.ToString());
                WriteEditOption(3, "Change unit", product.UnitId.ToString(), editedUnit.ToString());
                WriteEditOption(4, "Change price per unit", product.PricePerUnit.ToString("N2"), editedPrice.ToString("N2"));
                WriteEditOption(5, "Change quantity", product.Quantity.ToString("N2"), editedQuantity.ToString("N2"));
                Console.WriteLine("6. Save and exit");
                Console.WriteLine("7. Cancel and exit");
                Console.Write("Choose an option: ");

                string? input = Console.ReadLine();
                if (UserCancel(input))
                {
                    ShowMessage("Product edit was canceled. No changes were made. Returning to the main menu.", "warning");
                    return;
                }

                switch (input!.Trim())
                {
                    case "1":
                        string? name = ReadRequiredText("New product name");
                        if (name != null)
                            editedName = name;
                        break;

                    case "2":
                        int? categoryId = ReadCategoryId();
                        if (categoryId != null)
                            editedCategory = (Category)categoryId.Value;
                        break;

                    case "3":
                        int? unitId = ReadUnitId();
                        if (unitId != null)
                            editedUnit = (Unit)unitId.Value;
                        break;

                    case "4":
                        decimal? price = ReadDecimal("New price per unit");
                        if (price != null)
                            editedPrice = price.Value;
                        break;

                    case "5":
                        decimal? quantity = ReadDecimal("New quantity");
                        if (quantity != null)
                            editedQuantity = quantity.Value;
                        break;

                    case "6":
                        if (productManager.UpdateProduct(
                                product.Id,
                                editedName,
                                editedCategory,
                                editedUnit,
                                editedPrice,
                                editedQuantity))
                        {
                            ShowMessage($"Product '{editedName}' was updated successfully.", "success");
                        }
                        else
                        {
                            ShowMessage("The product could not be updated.", "error");
                        }

                        return;

                    case "7":
                        ShowMessage("Product edit was canceled. No changes were made. Returning to the main menu.", "warning");
                        return;

                    default:
                        ShowMessage("Invalid choice. Choose an option from 1 to 7.", "error");
                        break;
                }
            }
        }

        public static void DeleteProduct(ProductManager productManager)
        {
            while (true)
            {
                Console.Write("Enter the product ID to remove, 'v' to view products, or 'q' to cancel: ");
                string? input = Console.ReadLine();

                if (UserCancel(input))
                {
                    ShowMessage("Removing a product was cancelled. Returning to the main menu.", "info");
                    return;
                }

                if (input!.Trim().Equals("v", StringComparison.OrdinalIgnoreCase))
                {
                    ShowProductsForSelection(productManager);
                    continue;
                }

                if (!int.TryParse(input, out int id))
                {
                    ShowMessage("Invalid product ID. Enter a whole number, 'v', or 'q'.", "error");
                    continue;
                }

                Product? product = productManager.GetProductById(id);
                if (product == null)
                {
                    ShowMessage($"No product with ID {id} was found.", "warning");
                    continue;
                }

                while (true)
                {
                    Console.WriteLine(
                        $"Are you sure you want to delete {product.Id} - {product.Name}, " +
                        $"from category {product.CategoryId}? (y/n): ");
                    string? confirmation = Console.ReadLine();

                    if (UserCancel(confirmation))
                    {
                        ShowMessage("Product was not deleted. Returning to the main menu.", "info");
                        return;
                    }

                    if (confirmation!.Trim().Equals("y", StringComparison.OrdinalIgnoreCase))
                    {
                        productManager.DeleteProduct(product.Id);
                        ShowMessage($"Product with ID {product.Id} was deleted successfully.", "success");
                        return;
                    }

                    if (confirmation.Trim().Equals("n", StringComparison.OrdinalIgnoreCase))
                    {
                        ShowMessage("Product was not deleted. Returning to the main menu.", "info");
                        return;
                    }

                    ShowMessage("Invalid choice. Enter 'y' to delete or 'n' to return to the main menu.", "error");
                }
            }
        }

        // This method displays the current and edited values for a product attribute, highlighting any unsaved changes.
        private static void WriteEditOption(int number, string label, string currentValue, string editedValue)
        {
            Console.Write($"{number}. {label} [Current: ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(currentValue);
            Console.ResetColor();

            if (!string.Equals(currentValue, editedValue, StringComparison.Ordinal))
            {
                Console.Write(" -> New: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(editedValue);
                Console.ResetColor();
            }

            Console.WriteLine("]");
        }

        private static void ShowProductsForSelection(ProductManager productManager)
        {
            ShowMessage("Products", "info");

            WriteTableHeader(
                ("ID", 5),
                ("Name", 25),
                ("Category", 20),
                ("PPU", 15),
                ("Quantity", 15));

            foreach (Product product in productManager.GetProductsById())
            {
                Console.WriteLine(
                    "{0,-5} {1,-25} {2,-20} {3,-15:N2} {4,-15:N2}",
                    product.Id,
                    product.Name,
                    product.CategoryId,
                    product.PricePerUnit,
                    product.Quantity);
            }

            Console.WriteLine();
        }

        public static void ShowReports(ProductManager productManager)
        {
            List<MenuOption> reportOptions =
            [
                new("Total inventory value", () => ShowTotalInventoryValue(productManager)),
                new("Inventory levels summary", () => ShowInventoryLevelsSummary(productManager)),
                new("Quantity and value by category", () => ShowCategoryReport(productManager))
            ];

            Menu reportsMenu = new("Reports", reportOptions);
            reportsMenu.Show();
        }

        private static void ShowTotalInventoryValue(ProductManager productManager)
        {
            decimal totalValue = productManager.GetTotalInventoryValue();

            ShowMessage(
                $"Total inventory value: {totalValue:N2}",
                "statistics");
        }

        private static void ShowInventoryLevelsSummary(ProductManager productManager)
        {
            ShowMessage("Inventory levels summary", "statistics");
            Console.WriteLine($"Number of products: {productManager.GetProductCount()}");
            Console.WriteLine($"Total quantity in stock: {productManager.GetTotalQuantity():N2}");
            Console.WriteLine($"Total inventory value: {productManager.GetTotalInventoryValue():N2}");
            Console.WriteLine();
        }

        private static void ShowCategoryReport(ProductManager productManager)
        {
            ShowMessage("Inventory quantity and value by category", "statistics");

            WriteTableHeader(
                ("Category", 20),
                ("Quantity", 15),
                ("Total value", 15));

            foreach (Category category in Enum.GetValues<Category>())
            {
                Console.WriteLine(
                    $"{category,-20} " +
                    $"{productManager.GetTotalQuantityByCategory(category),-15:N2} " +
                    $"{productManager.GetTotalValueByCategory(category),-15:N2}");
            }

            Console.WriteLine();
        }

        public static void ShowProducts(ProductManager productManager)
        {
            List<MenuOption> viewOptions =
            [
                new("Show products by Category", () => ShowProductsByCategory(productManager)),
                new("Show Products by Last modified date", () => ShowProductsByLastModifiedDate(productManager)),
                new("Show Products by price per unit", () => ShowProductsByPrice(productManager)),
                new("Show products by quantity", () => ShowProductsByQuantity(productManager)),
                new("Show products by ID", () => ShowProductsById(productManager))
            ];

            Menu viewMenu = new("View Products", viewOptions);
            viewMenu.Show();
        }

        private static void ShowProductsByLastModifiedDate(ProductManager productManager)
        {
            ShowMessage("Products by last modified date", "info");
            WriteTableHeader(
                ("ID", 5),
                ("Name", 25),
                ("Category", 20),
                ("Last modified", 20),
                ("Quantity", 15),
                ("PPU", 15));

            foreach (Product product in productManager.GetProductsByLastModifiedDate())
            {
                string warningText = GetProductWarnings(product, includeLastModifiedWarning: true);
                SetWarningColor(warningText);

                Console.WriteLine(
                    "{0,-5} {1,-25} {2,-20} {3,-20:yyyy-MM-dd} {4,-15:N2} {5,-15:N2}{6}",
                    product.Id,
                    product.Name,
                    product.CategoryId,
                    product.LastModifiedOn,
                    product.Quantity,
                    product.PricePerUnit,
                    FormatWarnings(warningText));

                Console.ResetColor();
            }

            Console.WriteLine(new string('-', 105));
            Console.WriteLine();
        }

        private static void ShowProductsByPrice(ProductManager productManager)
        {
            ShowMessage("Products by price per unit", "info");
            WriteTableHeader(
                ("ID", 5),
                ("Name", 25),
                ("Category", 20),
                ("PPU", 15),
                ("Quantity", 15));

            foreach (Product product in productManager.GetProductsByPrice())
            {
                string warningText = GetProductWarnings(product);
                SetWarningColor(warningText);

                Console.Write(
                    "{0,-5} {1,-25} {2,-20} {3,-15:N2} {4,-15:N2}{5}",
                    product.Id,
                    product.Name,
                    product.CategoryId,
                    product.PricePerUnit,
                    product.Quantity,
                    FormatWarnings(warningText));

                Console.WriteLine();
                Console.ResetColor();
            }

            Console.WriteLine();
        }

        private static void ShowProductsByQuantity(ProductManager productManager)
        {
            ShowMessage("Products by quantity", "info");
            WriteTableHeader(
                ("ID", 5),
                ("Name", 25),
                ("Category", 20),
                ("Quantity", 15),
                ("PPU", 15));

            foreach (Product product in productManager.GetProductsByQuantity())
            {
                string warningText = GetProductWarnings(product);
                SetWarningColor(warningText);

                Console.Write(
                    "{0,-5} {1,-25} {2,-20} {3,-15:N2} {4,-15:N2}{5}",
                    product.Id,
                    product.Name,
                    product.CategoryId,
                    product.Quantity,
                    product.PricePerUnit,
                    FormatWarnings(warningText));

                Console.WriteLine();
                Console.ResetColor();
            }

            Console.WriteLine();
        }

        private static void ShowProductsById(ProductManager productManager)
        {
            ShowMessage("Products by ID", "info");
            WriteTableHeader(
                ("ID", 5),
                ("Name", 25),
                ("Category", 20),
                ("PPU", 15),
                ("Quantity", 15),
                ("Total price", 15));

            foreach (Product product in productManager.GetProductsById())
            {
                string warningText = GetProductWarnings(product);
                SetWarningColor(warningText);

                Console.WriteLine(
                    "{0,-5} {1,-25} {2,-20} {3,-15:N2} {4,-15:N2} {5,-15:N2}{6}",
                    product.Id,
                    product.Name,
                    product.CategoryId,
                    product.PricePerUnit,
                    product.Quantity,
                    product.TotalPrice,
                    FormatWarnings(warningText));

                Console.ResetColor();
            }

            Console.WriteLine();
        }

        private static void ShowProductsByCategory(ProductManager productManager)
        {
            foreach (Category category in Enum.GetValues(typeof(Category)))
            {
                Console.WriteLine();
                ShowMessage($"Category: {category}", "statistics");

                WriteTableHeader(
                    ("ID", 5),
                    ("Name", 25),
                    ("Category", 20),
                    ("Quantity", 15),
                    ("PPU", 15),
                    ("Total price", 15),
                    ("Added on date", 16),
                    ("Last modified on", 16));

                List<Product> products = productManager.GetProductsByCategory(category);

                if (products.Count == 0)
                {
                    Console.WriteLine("No products found for this category.");
                }

                foreach (Product product in products)
                {
                    string warningText = GetProductWarnings(product);
                    SetWarningColor(warningText);

                    Console.WriteLine(
                        "{0,-5} {1,-25} {2,-20} {3,-15:N2} {4,-15:N2} {5,-15:N2} {6,-16:yyyy-MM-dd} {7,-20:yyyy-MM-dd}{8}",
                        product.Id,
                        product.Name,
                        product.CategoryId,
                        product.Quantity,
                        product.PricePerUnit,
                        product.TotalPrice,
                        product.AddedOn,
                        product.LastModifiedOn,
                        FormatWarnings(warningText));

                    Console.ResetColor();
                }

                Console.WriteLine(new string('-', 134));
            }
        }

        private static string GetProductWarnings(Product product, bool includeLastModifiedWarning = false)
        {
            List<string> warnings = [];

            if (product.PricePerUnit == 0)
                warnings.Add("!: No price per unit set");

            if (product.Quantity >= 0 && product.Quantity <= 5)
                warnings.Add("!: Low quantity");

            if (includeLastModifiedWarning && product.LastModifiedOn < DateTime.Today.AddMonths(-12))
                warnings.Add("!: Not modified in over 12 months");

            return string.Join(" | ", warnings);
        }

        private static void SetWarningColor(string warningText)
        {
            if (!string.IsNullOrEmpty(warningText))
                Console.ForegroundColor = ConsoleColor.Red;
        }

        private static string FormatWarnings(string warningText)
        {
            return string.IsNullOrEmpty(warningText) ? string.Empty : $" {warningText}";
        }

    }
}