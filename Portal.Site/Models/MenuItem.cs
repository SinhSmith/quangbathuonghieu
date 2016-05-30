using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Site.Models
{
    public class MenuItem
    {
        public MenuItem()
        {
            this.ChildMenuItems = new List<MenuItem>();
        }

        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public string MenuItemPath { get; set; }
        public string IconClass { get; set; }
        public Nullable<int> ParentItemId { get; set; }
        public virtual ICollection<MenuItem> ChildMenuItems { get; set; }
    }
}