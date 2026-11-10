using inventory.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace inventory.Services
{
    public class ProductManager
    {
        public List<Product> _products = new List<Product>();
        public ProductManager()
        {
            // Initialize the product list with some sample products
            _products.Add(new Product(1, "Pen", 1.5, 5, 100, 1));
            _products.Add(new Product(2, "Notebook", 3.0, 1, 50, 1));
            _products.Add(new Product(3, "Printer Paper", 5.0, 2, 200, 3));
        }
        public List<Product> GetProductsByCategory(Category category)
        {
            throw new NotImplementedException();
        }

        public void AddProduct()
        {
            throw new NotImplementedException();
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

        public void Remove()
        {
            throw new NotImplementedException();
        }

        public void Reports()
        {
            throw new NotImplementedException();
        }
    }
}
