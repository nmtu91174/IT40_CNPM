using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NhacCuaTui.Models;

namespace NhacCuaTui.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        // GET: Admin/Users
        public ActionResult Index()
        {
            if (Session["Role"] == null)
            {
                return Redirect("~/Login/Login");
            }
            if (Session["Role"].Equals("Admin"))
            {
                DataModel db = new DataModel();
                ViewBag.listUsers = db.get("EXEC GetAllUsers;");
                return View();
            }
            else
            {
                return Redirect("~/Home/Index");
            }
        }

        public ActionResult AddUser()
        {
            if (Session["Role"] == null)
            {
                return Redirect("~/Login/Login");
            }
            if (Session["Role"].Equals("Admin"))
            {
                return View();
            }
            else
            {
                return Redirect("~/Home/Index");
            }
        }

        [HttpPost]
        public ActionResult ThemNguoiDung(string username,
                                          string password,
                                          string fullname,
                                          string phonenumber,
                                          string email,
                                          string role)
        {
            if (Session["Role"].Equals("Admin") == false)
            {
                return Redirect("~/Home/Index");
            }
            try
            {
                DataModel db = new DataModel();
                db.get("EXEC AddNewUser " +
                        "@Username = '" + username + "', " +
                        "@Password = '" + password + "', " +
                        "@Fullname = N'" + fullname + "', " +
                        "@Role = N'" + role + "', " +
                        "@Email = '" + email + "', " +
                        "@PhoneNumber = '" + phonenumber + "'; ");

                return RedirectToAction("Index", "Users", "Admin");
            }
            catch (Exception) { }
            Session["AddUser_Error"] = "Đã tồn tại tên đăng nhập hoặc email!";
            return RedirectToAction("AddUser", "Users", "Admin");
        }

        public ActionResult EditUser(string id)
        {
            if (Session["Role"] == null)
            {
                return Redirect("~/Login/Login");
            }
            if (Session["Role"].Equals("Admin"))
            {
                DataModel db = new DataModel();
                ViewBag.GetUserById = db.get("EXEC GetUserById @UserId = " + id + ";");
                return View();
            }
            else
            {
                return Redirect("~/Login/Login");
            }
        }

        [HttpPost]
        public ActionResult SuaNguoiDung(string id,
                                          string fullname,
                                          string phonenumber,
                                          string email,
                                          string role)
        {
            if (Session["Role"].Equals("Admin") == false)
            {
                return Redirect("~/Home/Index");
            }
            try
            {
                DataModel db = new DataModel();
                db.get("EXEC UpdateUser "
                        + "@UserId = " + id + ","
                        + "@Email = '" + email + "',"
                        + "@PhoneNumber = '" + phonenumber + "',"
                        + "@Fullname = N'" + fullname + "',"
                        + "@Role = N'" + role + "'");

                return RedirectToAction("Index", "Users", "Admin");
            }
            catch (Exception) { }
            Session["AddUser_Error"] = "Đã tồn tại email!";
            return RedirectToAction("EditUser", "Users", "Admin");
        }

        public ActionResult XoaNguoiDung(string id)
        {
            if (Session["Role"].Equals("Admin") == false)
            {
                return Redirect("~/Home/Index");
            }
            if (Session["Role"] == null)
            {
                return Redirect("~/Login/Login");
            }
            if (Session["Role"].Equals("Admin"))
            {
                DataModel db = new DataModel();
                db.get("EXEC DeleteUser @UserId = " + id + ";");
                return RedirectToAction("Index", "Users", "Admin");
            }
            else
            {
                return Redirect("~/Home/Index");
            }
        }
    }
}