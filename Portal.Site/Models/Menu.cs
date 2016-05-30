using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Site.Models
{
    public class Menu
    {
        public Menu()
        {
            Items = new List<MenuItem>();
        }

        public List<MenuItem> Items;
    }
}