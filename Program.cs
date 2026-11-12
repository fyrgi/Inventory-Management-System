using inventory;
using inventory.Models;
using inventory.Presentation;
using inventory.Services;



Console.WriteLine("Little Stationery Shop's Inventory Management System Beta");

// ToDo:
// UML Activity Diagram - OK
// Menu
// Class Product (id, name, price, quantity, category, category and more) OK
// Enum with categories (no option for delete or edit, but maybe user can add?) OK
// Plan where methods will be (change in quantity, price, add a discount). ProductService OK
// Validations
// Error handlig

string dataFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\Data"));
FileHandler fileHandler = new(dataFolder, "Products.json");
fileHandler.EnsureFileExists();
List<Product> productsFromFile = fileHandler.ReadJson<Product>();
ProductManager productManager = new ProductManager(productsFromFile, fileHandler);

List<(int DuplicateId, int NewId)> idRepairs = productManager.RepairDuplicateIds();
if (idRepairs.Count > 0)
{
    UI.ShowMessage(
        $"Warning: {idRepairs.Count} duplicate product ID(s) were found.",
        "warning");

    productManager.SaveToFile();

    foreach ((int duplicateId, int newId) in idRepairs)
    {
        UI.ShowMessage(
            $"Product with duplicate ID {duplicateId} was assigned new ID {newId}.",
            "info");
    }

    UI.ShowMessage("Duplicate product IDs were repaired and saved.", "success");
}


//var productManager = new ProductManager();

List<MenuOption> mainMenuOptions =
[
    new("Add Product", () => UI.AddProduct(productManager)),
    new("View Products", () => UI.ShowProducts(productManager)),
    new("Search Products", () => UI.SearchProducts(productManager)),
    new("Edit Product", () => UI.EditProduct(productManager)),
    new("Remove Product", () => UI.DeleteProduct(productManager)),
    new("Reports", () => UI.ShowReports(productManager))
];

// Under Reports I want to start a new menu where the user can 
Menu menu = new("Main Menu", mainMenuOptions);

menu.Show();