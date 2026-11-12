using inventory.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace inventory.Services
{
    public class ProductManager
    {
        private readonly List<Product> _products;
        private readonly FileHandler _fileHandler;
        public int NextId => _products.Count == 0
            ? 1
            : _products.Max(product => product.Id) + 1;

        public ProductManager(List<Product> products, FileHandler fileHandler)
        {
            _products = new List<Product>(products);
            _fileHandler = fileHandler;
        }
        public List<Product> GetProductsByCategory(Category category)
        {
            return _products
                .Where(product => product.CategoryId == category)
                .OrderBy(product => product.Id)
                .ToList();
        }

        public Product? GetProductById(int id)
        {
            return _products.Find(product => product.Id == id);
        }

        public List<Product> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return [];

            return _products
                .Where(product => ContainsSearchTerm(product.Id.ToString(), searchTerm) ||
                                 ContainsSearchTerm(product.Name, searchTerm) ||
                                 ContainsSearchTerm(product.CategoryId.ToString(), searchTerm))
                .OrderBy(product => product.Id)
                .ToList();
        }

        public List<Product> GetProductsByLastModifiedDate()
        {
            return _products
                .OrderByDescending(product => product.LastModifiedOn)
                .ToList();
        }

        public List<Product> GetProductsByPrice()
        {
            return _products
                .OrderBy(product => product.PricePerUnit)
                .ThenBy(product => product.Name)
                .ToList();
        }

        public List<Product> GetProductsByQuantity()
        {
            return _products
                .OrderBy(product => product.Quantity)
                .ToList();
        }

        public List<Product> GetProductsById()
        {
            return _products
                .OrderBy(product => product.Id)
                .ToList();
        }

        private static bool ContainsSearchTerm(string value, string searchTerm)
        {
            return value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
        }

        public List<(int DuplicateId, int NewId)> RepairDuplicateIds()
        {
            HashSet<int> usedIds = [];
            List<(int DuplicateId, int NewId)> repairs = [];

            foreach (Product product in _products)
            {
                if (usedIds.Add(product.Id))
                    continue;

                int duplicateId = product.Id;
                int newId = NextId;
                product.Id = newId;
                usedIds.Add(newId);
                repairs.Add((duplicateId, newId));
            }

            return repairs;
        }

        public int GetProductCount()
        {
            return _products.Count;
        }

        public decimal GetTotalQuantity()
        {
            return _products.Sum(product => product.Quantity);
        }

        public decimal GetTotalInventoryValue()
        {
            return _products.Sum(product => product.TotalPrice);
        }

        public decimal GetTotalQuantityByCategory(Category category)
        {
            return GetProductsByCategory(category).Sum(product => product.Quantity);
        }

        public decimal GetTotalValueByCategory(Category category)
        {
            return GetProductsByCategory(category).Sum(product => product.TotalPrice);
        }

        public void AddProduct(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Product name cannot be empty.", nameof(product));

            if (product.PricePerUnit < 0 || product.Quantity < 0)
                throw new ArgumentException("Product price and quantity cannot be negative.", nameof(product));

            _products.Add(product);
            SaveToFile();
        }

        public bool UpdateProduct(int id, string name, Category category, Unit unit, decimal pricePerUnit, decimal quantity)
        {
            Product? product = GetProductById(id);
            if (product == null)
                return false;

            if (string.IsNullOrWhiteSpace(name) 
                || pricePerUnit < 0 
                || quantity < 0
                || !Enum.IsDefined(category)
                || !Enum.IsDefined(unit))
            {
                return false;
            }

            product.Name = name;
            product.CategoryId = category;
            product.UnitId = unit;
            product.PricePerUnit = pricePerUnit;
            product.Quantity = quantity;
            product.TotalPrice = pricePerUnit * quantity;
            product.LastModifiedOn = DateTime.Now;

            SaveToFile();
            return true;
        }

        public void SaveToFile()
        {
            _fileHandler.WriteJson(_products);
        }

        public bool DeleteProduct(int id)
        {
            Product? product = _products.Find(product => product.Id == id);
            if (product == null)
                return false;

            _products.Remove(product);
            SaveToFile();
            return true;
        }

    }
}
