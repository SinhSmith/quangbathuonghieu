﻿using Portal.Core.Database;
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
            if (Request.QueryString["city"] != null)
            {
                ViewBag.City = new SelectList(cities, "Id", "Name", Request.QueryString["city"].ToString());
            }
            else
            {
                ViewBag.City = new SelectList(cities, "Id", "Name");
            }

            var trades = Core.Service.CategoryService.GetCategoryByType((int)Core.Service.BaseService.CategoryType.Trades);
            if (Request.QueryString["trade"] != null)
            {
                ViewBag.Trade = new SelectList(trades, "Id", "Name", Request.QueryString["trade"].ToString());
            }
            else
            {
                ViewBag.Trade = new SelectList(trades, "Id", "Name");
            }

            if (Request.QueryString["keyword"] != null)
                ViewBag.keyword = Request.QueryString["keyword"].ToString();

            return PartialView();
        }

        public ActionResult BannerPartial()
        {
            var banners = db.Banners.Where(x => x.Status == (int)Portal.Core.Util.Define.Status.Active).OrderBy(x => x.SortOrder);
            return PartialView(banners.ToList());
        }

        public ActionResult CountUserOnlinePartial()
        {
            return PartialView();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
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
                _menu4.ChildMenuItems.Add(new MenuItem()
                {
                    MenuItemName = "Quản lý banner",
                    MenuItemPath = "/quan-ly-banner",
                    IconClass = "glyphicon-th-list"
                });

                _menu.Items.Add(_menu4);
            }

            return PartialView("_Menu", _menu);
        }
    }
}