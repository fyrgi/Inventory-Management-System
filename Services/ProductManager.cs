using inventory.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace inventory.Services
{
    public class ProductManager
    {
        public List<Product> _products = new List<Product>();
        private readonly FileHandler _fileHandler;
        public int NextId => _products.Count == 0
            ? 1
            : _products.Max(product => product.Id) + 1;

        public ProductManager(List<Product> products, FileHandler fileHandler)
        {
            _products = products;
            _fileHandler = fileHandler;
        }
        public List<Product> GetProductsByCategory(Category category)
        {
            return _products.FindAll(p => p.CategoryId == category);
            
        }

        public List<Product> GetProducts()
        {
            return _products;
        }

        public void AddProduct(Product product)
        {
            _products.Add(product);
        }

        public void SaveToFile()
        {

            _fileHandler.WriteJson(_products);
        }

        public void View()
        {
            throw new NotImplementedException();
        }

        public void Search()
        {
            throw new NotImplementedException();
        }

        public void EditProduct()
        {
            throw new NotImplementedException();
        }


        // Takes the ID of a product and removes it from the file Products.json (and from the _products list in memory).
        public void Remove()
        {
            Console.Write("Enter the ID of the product to remove: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID. Please enter a valid integer.");
                return;
            }
            var product = _products.Find(p => p.Id == id);
            if (product != null)
            {
                _products.Remove(product);
                _fileHandler.WriteJson(_products);
            }
            else
            {
                Console.WriteLine($"Product with ID {id} not found.");
            }


        }

        public void Reports()
        {
            throw new NotImplementedException();
        }
    }
}
