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


//var productManager = new ProductManager();

List<MenuOption> mainMenuOptions =
[
    new("Add Product", () => UI.AddProduct(productManager)),
    new("View Products", () => UI.ShowProducts(productManager)),
    new("Search Products", productManager.Search),
    new("Edit Product", productManager.EditProduct),
    new("Remove Product", productManager.Remove),
    new("Reports", productManager.Reports)
];

Menu menu = new("Main Menu", mainMenuOptions);
menu.Show();