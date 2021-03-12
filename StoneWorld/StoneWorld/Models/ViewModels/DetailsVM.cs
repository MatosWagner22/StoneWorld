using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoneWorld.Models.ViewModels
{
    public class DetailsVM
    {
        public DetailsVM()
        {
            Product = new Product();//Singleton
        }

        public Product Product { get; set; }
        public bool ExistsInCart { get; set; }
    }
}
