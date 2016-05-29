using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Core.Service;
using Portal.CMS.Models;
using Microsoft.AspNet.Identity;
using Portal.Core.Util;

namespace Portal.CMS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
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

        public ActionResult List()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Error404()
        {
            ViewBag.Message = "Error";

            return View();

        }
        public ActionResult MessageDisplay()
        {

            return View();
        }

        [ChildActionOnly]
        public ActionResult Menu()
        {
            var _menu = new Menu();

            // Normally you'd do some data or cache access to build/retrieve the user's menu
            // then you're loop through the results and build the menu object
            // we're hard coding for the sake of simplicity

            //var _google = new MenuItem()
            //{
            //    MenuItemName = "Google",
            //    MenuItemPath = "http://google.com/",
            //};

            //_google.ChildMenuItems.Add(new MenuItem()
            //{
            //    MenuItemName = "Google Images",
            //    MenuItemPath = "http://google.com/images/"
            //});

            //var _bing = new MenuItem()
            //{
            //    MenuItemName = "Bing",
            //    MenuItemPath = "http://bing.com/"
            //};

            //_bing.ChildMenuItems.Add(new MenuItem()
            //{
            //    MenuItemName = "Bing Images",
            //    MenuItemPath = "http://bing.com/images/"
            //});

            //_menu.Items.Add(_google);
            //_menu.Items.Add(_bing);

            //Portal.Core.Database.Profile profile = ProfileService.GetProfile(User.Identity.GetUserId());
            var _menu1 = new MenuItem()
            {
                MenuItemName = "Trang chủ",
                MenuItemPath = "/",
            };
            _menu.Items.Add(_menu1);
            
            //if (profile != null && (profile.Role == (int)Define.UserRole.Candidates || User.IsInRole("Administrator")))           
            {
                var _menu2 = new MenuItem()
                {
                    MenuItemName = "Ứng viên",
                    MenuItemPath = "#",
                };
                _menu2.ChildMenuItems.Add(new MenuItem()
                {
                    MenuItemName = "Hồ sơ ứng viên",
                    MenuItemPath = "/ho-so-ung-vien",
                    IconClass = "glyphicon-edit"
                });
                //_menu2.ChildMenuItems.Add(new MenuItem()
                //{
                //    MenuItemName = "Công việc đã lưu",
                //    MenuItemPath = "#",
                //    IconClass = "glyphicon-floppy-saved"
                //});
                //_menu2.ChildMenuItems.Add(new MenuItem()
                //{
                //    MenuItemName = "Các việc đã ứng tuyển",
                //    MenuItemPath = "#",
                //    IconClass = "glyphicon-export"
                //});
                _menu2.ChildMenuItems.Add(new MenuItem()
                {
                    MenuItemName = "Tài khoản của tôi",
                    MenuItemPath = "/quan-ly-tai-khoan",
                    IconClass = "glyphicon-user"
                });

                _menu.Items.Add(_menu2);
            }

            //if (profile != null && (profile.Role == (int)Define.UserRole.Employer || User.IsInRole("Administrator")))
            if (User.IsInRole("Administrator"))
            {
                var _menu3 = new MenuItem()
                {
                    MenuItemName = "Nhà tuyển dụng",
                    MenuItemPath = "#",
                };
                _menu3.ChildMenuItems.Add(new MenuItem()
                {
                    MenuItemName = "Đăng tuyển dụng",
                    MenuItemPath = "/dang-tuyen-dung",
                    IconClass = "glyphicon-plus"
                });
                _menu3.ChildMenuItems.Add(new MenuItem()
                {
                    MenuItemName = "Danh sách tuyển dụng",
                    MenuItemPath = "/danh-sach-tuyen-dung",
                    IconClass = "glyphicon-th-list"
                });
                _menu3.ChildMenuItems.Add(new MenuItem()
                {
                    MenuItemName = "Hồ sơ doanh nghiệp",
                    MenuItemPath = "/ho-so-doanh-nghiep",
                    IconClass = "glyphicon-edit"
                });
                _menu3.ChildMenuItems.Add(new MenuItem()
                {
                    MenuItemName = "Tài khoản của tôi",
                    MenuItemPath = "/quan-ly-tai-khoan",
                    IconClass = "glyphicon-user"
                });
                _menu.Items.Add(_menu3);

            }

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