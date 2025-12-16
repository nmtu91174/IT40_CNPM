using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NhacCuaTui.Models;

namespace NhacCuaTui.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LoginAccount(string username, string password)
        {
            try
            {
                DataModel db = new DataModel();
                ViewBag.Login = db.get("exec UserLogin @Username = '" + username + "'," + "@Password = '" + password + "';");
                var loginList = ViewBag.Login as ArrayList;
                if (loginList != null && loginList.Count > 0)
                {
                    var firstRow = loginList[0] as ArrayList;
                    if (firstRow != null && firstRow.Count >= 6)
                    {
                        // Lưu thông tin người dùng vào Session
                        Session["userId"] = firstRow[0];
                        Session["username"] = firstRow[1];
                        Session["email"] = firstRow[2];
                        Session["phone"] = firstRow[3];
                        Session["fullname"] = firstRow[4];
                        Session["Role"] = firstRow[5];
                        Session["Password"] = password;

                        // Điều hướng dựa trên vai trò người dùng
                        string role = Session["Role"]?.ToString() ?? string.Empty;
                        if (role.Equals("Nhân viên") || role.Equals("Người dùng"))
                        {
                            return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
                        }
                        else if (role.Equals("Admin"))
                        {
                            return Json(new { success = true, redirectUrl = Url.Action("Index", "Dashboard", new { area = "Admin" }) });
                        }
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Tên đăng nhập hoặc mật khẩu không đúng." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }

            return Json(new { success = false, message = "Đăng nhập thất bại." });
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegisterAccount(string username,
                                    string password,
                                    string email,
                                    string phone,
                                    string fullname)
        {
            DataModel db = new DataModel();
            ViewBag.listUsers = db.get("EXEC GetAllUsers;");

            var existingUser = db.get("EXEC CheckUserExists @Username = '" + username + "';");
            // CheckUserExists returns a single row with 1 (exists) or 0 (not exists)
            if (existingUser != null && existingUser.Count > 0)
            {
                var firstRow = existingUser[0] as ArrayList;
                var flag = firstRow != null && firstRow.Count > 0 ? firstRow[0]?.ToString() : null;
                if (flag == "1")
                {
                    return Json(new { success = false, message = "Tên đăng nhập đã tồn tại." });
                }
            }

            try
            {
                // Basic input validation
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(email))
                {
                    return Json(new { success = false, message = "Vui lòng nhập đầy đủ thông tin đăng ký." });
                }

                db.get("EXEC RegisterUser @Username = '" + username + "', " +
                        "@Password = '" + password + "', " +
                        "@Email = '" + email + "', " +
                        "@PhoneNumber = '" + phone + "', " +
                        "@Fullname = N'" + fullname + "'");

                // Attempt auto-login after successful registration
                ViewBag.Login = db.get("exec UserLogin @Username = '" + username + "'," + "@Password = '" + password + "';");
                var loginList2 = ViewBag.Login as ArrayList;
                if (loginList2 != null && loginList2.Count > 0)
                {
                    var firstRow2 = loginList2[0] as ArrayList;
                    if (firstRow2 != null && firstRow2.Count >= 6)
                    {
                        Session["userId"] = firstRow2[0];
                        Session["username"] = firstRow2[1];
                        Session["email"] = firstRow2[2];
                        Session["phone"] = firstRow2[3];
                        Session["fullname"] = firstRow2[4];
                        Session["Role"] = firstRow2[5];
                        Session["Password"] = password;
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

        public ActionResult LogOut()
        {
            Session["userId"] = null;
            Session["username"] = null;
            Session["email"] = null;
            Session["phone"] = null;
            Session["fullname"] = null;
            Session["Role"] = null;
            Session["Password"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}