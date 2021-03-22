using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoneWorld.Models.ViewModels
{
    public class ProductUserVM
    {
        public ProductUserVM()
        {
            ProductList = new List<Product>(); // singleton
        }

        public ApplicationUser ApplicationUser  { get; set; }
        public IEnumerable<Product> ProductList { get; set; }
    }
}
