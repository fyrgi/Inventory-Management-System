# Inventory Management System

A console-based inventory management system for a small stationery shop. The application stores products in a JSON file and provides menus for adding, viewing, searching, editing, removing, and reporting on products.

## Project idea

The main design goal of this project was to use a clean separation of responsibilities:

- **Presentation** handles menus, console input, table output, colors, and messages.
- **Services** contains product operations, validation, searching, sorting, calculations, and persistence coordination.
- **Models** contains the product data and enums used by the application.
- **Data** contains the JSON inventory file.

Following this structure caused some challenges during development because console workflows, business rules, and file operations must be kept separate. For example, the UI collects values when a product is edited, but `ProductManager` is responsible for validating and applying the update. This separation makes the code easier to maintain and allows the product logic to be reused by another interface in the future.

## Main functionality

### Add Product

The user can add a product by entering:

- Product name
- Category
- Price per unit
- Quantity
- Unit

The product receives the next available ID. Its total price is calculated from:

```text
Total price = Price per unit × Quantity
```

The product is then saved to `Data/Products.json`.

### View Products

The View Products submenu provides several ways to inspect the inventory:

- **By Category** - groups products under a clear category heading.
- **By Last Modified Date** - shows the most recently modified products first.
- **By Price Per Unit** - orders products by price; products with equal prices are ordered by name.
- **By Quantity** - orders products from the lowest quantity to the highest.
- **By ID** - orders products from the lowest ID to the highest.

Product tables display relevant details such as ID, name, category, quantity, unit, price per unit, total price, and dates where applicable.

Warnings are displayed consistently in the product views:

- Products with no price per unit are marked in red.
- Products with a quantity from 0 through 5 are marked in red as low quantity.
- Products not modified for more than 12 months are marked in red in the last-modified view.

Dates are displayed as `yyyy-MM-dd`.

### Search Products

Products can be searched by a text value matching:

- Product ID
- Product name
- Category name

The search is case-insensitive and supports partial matches. For example, searching for `5` can match an ID containing `5`, a product name containing `5`, or a category name containing `5`. The numeric category ID is not searched as a category name.

Matching text is highlighted in purple in the result table so the user can see whether the match came from the ID, product name, category name, or more than one field.

### Edit Product

The user selects a product by ID and can edit:

1. Name
2. Category
3. Unit
4. Price per unit
5. Quantity

Changes are held as pending values until the user chooses **Save and exit**. The edit menu shows the saved current value and, when changed, the new pending value.

When changes are saved:

- The product values are updated.
- `TotalPrice` is recalculated.
- `LastModifiedOn` is updated.
- The updated inventory is written to `Products.json`.

Choosing **Cancel and exit** discards all pending changes and leaves the file unchanged.

### Remove Product

The user enters a product ID and confirms the deletion with `y` or `n`. The `v` option displays a simple product list before selection, and `q` cancels the operation.

After confirmation, the product is removed from the in-memory inventory and the updated list is saved to `Products.json`.

## Reports

The Reports submenu contains three reports.

### Total inventory value

Calculates the value of all products currently in stock:

```text
Sum of every product's TotalPrice
```

### Inventory levels summary

Displays:

- Number of products
- Total quantity in stock
- Total inventory value

### Quantity and value by category

Displays each category with:

- Total quantity in the category
- Total value of products in the category

## Product IDs

Product IDs are managed by `ProductManager`.

### New products

When a product is added, its ID is assigned using the next available ID:

```text
Next ID = highest existing ID + 1
```

If the inventory is empty, the first ID is `1`.

### Duplicate IDs on startup

When the application starts, it checks the products loaded from `Products.json` for duplicate IDs.

- The first product using an ID keeps it.
- Later products with the same ID receive new IDs using `NextId`.
- The user is shown a warning.
- Each repaired ID is reported.
- The corrected data is saved back to the JSON file.

This ensures that product IDs are unique before the user begins working with the inventory. Unique IDs are important because editing and deleting products are both performed by ID.

## Data storage

Inventory data is stored in:

```text
Data/Products.json
```

`FileHandler` is responsible for:

- Creating the file and directory when necessary.
- Reading JSON data into product objects.
- Serializing product objects back to JSON.

The application uses short-lived file access for each read and write. The JSON file should be treated as application data and should not be edited manually while the application is running.

## Requirements

- .NET 10 SDK
- A terminal or an IDE such as Visual Studio
- Write permission for the project directory so the application can update `Data/Products.json`

The project targets:

```xml
<TargetFramework>net10.0</TargetFramework>
```

## Running the project

From the project directory, restore and run the application with:

```powershell
dotnet restore
dotnet run --project inventory.csproj
```

To build without running:

```powershell
dotnet build inventory.csproj
```

The application starts by loading `Data/Products.json`, repairing duplicate IDs if necessary, and then displaying the main menu.

## Project structure

```text
Inventory-Management-System/
├── Data/
│   └── Products.json
├── Models/
│   ├── Category.cs
│   ├── Product.cs
│   └── Unit.cs
├── Presentation/
│   ├── Menu.cs
│   ├── MenuOption.cs
│   └── UI.cs
├── Services/
│   ├── FileHandler.cs
│   └── ProductManager.cs
├── Program.cs
└── inventory.csproj
```
