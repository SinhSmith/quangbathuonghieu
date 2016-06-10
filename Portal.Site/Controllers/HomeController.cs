using Portal.Core.Database;
using Portal.Site.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Site.Controllers
{
    public class HomeController : Controller
    {
        private PortalEntities db = new PortalEntities();
        public ActionResult Index()
        {
            ViewBag.Message = "Your application home page.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult SearchPartial()
        {
            var cities = Core.Service.CategoryService.GetCategoryByType((int)Core.Service.BaseService.CategoryType.City);
            ViewBag.City = new SelectList(cities, "Id", "Name");

            var trades = Core.Service.CategoryService.GetCategoryByType((int)Core.Service.BaseService.CategoryType.Trades);
            ViewBag.Trade = new SelectList(trades, "Id", "Name");

            return PartialView();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [ChildActionOnly]
        public ActionResult Menu()
        {
            var _menu = new Menu();

            var _menu1 = new MenuItem()
            {
                MenuItemName = "Trang chủ",
                MenuItemPath = "/",
            };
            _menu.Items.Add(_menu1);

            if (User.IsInRole("Administrator"))
            {
                var _menu4 = new MenuItem()
                {
                    MenuItemName = "Quản trị",
                    MenuItemPath = "#",
                };
                _menu4.ChildMenuItems.Add(new MenuItem()
                {
                    MenuItemName = "Quản lý danh mục doanh nghiệp",
                    MenuItemPath = "/quan-ly-doanh-nghiep",
                    IconClass = "glyphicon-th-list"
                });
                _menu4.ChildMenuItems.Add(new MenuItem()
                {
                    MenuItemName = "Quản lý danh mục sản phẩm",
                    MenuItemPath = "/quan-ly-san-pham",
                    IconClass = "glyphicon-th-list"
                });
                _menu4.ChildMenuItems.Add(new MenuItem()
                {
                    MenuItemName = "Quản lý người dùng",
                    MenuItemPath = "/quan-ly-nguoi-dung",
                    IconClass = "glyphicon-th-list"
                });

                _menu.Items.Add(_menu4);
            }

            return PartialView("_Menu", _menu);
        }
    }
}