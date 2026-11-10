using System;
using System.Collections.Generic;
using System.Text;

namespace inventory.Models
{
    public class Product
    {
        private static int _currentId;
        public Product() { }
        public Product(int id, string name, double pricePerUnit, int category, double quantity, int unitId )
        {
            Id = id;
            CategoryId = (Models.Category)category;
            UnitId = (Models.Unit)unitId;
            Name = name;
            PricePerUnit = pricePerUnit;
            Quantity = quantity;
            TotalPrice = pricePerUnit * quantity;
            AddedOn = DateTime.Now;
            LastModifiedOn = DateTime.Now;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public double PricePerUnit { get; set; }
        public Models.Category CategoryId { get; set; }
        public Models.Unit UnitId { get; set; }
        public double Quantity { get; set; }
        public double TotalPrice { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime LastModifiedOn { get; set; }
    }
}
