using Portal.Core.Database;
using Portal.Site.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace Portal.Site.Controllers
{
    public class HomeController : Controller
    {
        private PortalEntities db = new PortalEntities();
        public ActionResult Index(string keyword, int page = 1)
        {
            var companies = db.Companies.Where(x => string.IsNullOrEmpty(keyword) || x.Name.Contains(keyword)).OrderByDescending(x => x.CreatedDate);
            return View(companies.ToList().ToPagedList(page, 10));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
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
                    MenuItemName = "Quản lý tin tuyển dụng",
                    MenuItemPath = "/quan-ly-tin-tuyen-dung",
                    IconClass = "glyphicon-plus"
                });
                _menu4.ChildMenuItems.Add(new MenuItem()
                {
                    MenuItemName = "Duyệt tin tuyển dụng",
                    MenuItemPath = "/danh-sach-tuyen-dung",
                    IconClass = "glyphicon-th-list"
                });
                _menu4.ChildMenuItems.Add(new MenuItem()
                {
                    MenuItemName = "Quản lý nhà tuyển dụng",
                    MenuItemPath = "/quan-ly-nha-tuyen-dung",
                    IconClass = "glyphicon-th-list"
                });
                _menu4.ChildMenuItems.Add(new MenuItem()
                {
                    MenuItemName = "Duyệt nhà tuyển dụng",
                    MenuItemPath = "/duyet-nha-tuyen-dung",
                    IconClass = "glyphicon-th-list"
                });

                _menu.Items.Add(_menu4);
            }

            return PartialView("_Menu", _menu);
        }
    }
}