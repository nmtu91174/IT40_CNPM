using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NhacCuaTui.Models;

namespace NhacCuaTui.Controllers
{
    public class UserController : Controller
    {
        DataModel db = new DataModel();
        // GET: User
        public ActionResult Index()
        {
            if (Session["Role"] == null)
            {
                return Redirect("~/Login/Login");
            }
            ViewBag.Top10BXH = db.get("EXEC GetTop10SongsByViews;");
            ViewBag.Top6CaSi = db.get("EXEC GetTop6ArtistsWithRanking;");
            ViewBag.GetAllSong = db.get("EXEC GetAllSongs;");
            int userId;
            if (!int.TryParse(Session["userId"]?.ToString(), out userId))
            {
                return Redirect("~/Home/Index");
            }

            var userInfo = db.get($"EXEC GetUserInfo @UserId = {userId}");
            ViewBag.getInfoUser = userInfo;

            return View();
        }

        public ActionResult EditUser(string id,
                                    string fullname,
                                    string email,
                                    string phone)
        {
            try
            {
                DataModel db = new DataModel();
                string role = Session["Role"].ToString();
                db.get("EXEC UpdateUser "
                        + "@UserId = " + id + ","
                        + "@Email = '" + email + "',"
                        + "@PhoneNumber = '" + phone + "',"
                        + "@Fullname = N'" + fullname + "',"
                        + "@Role = N'" + role + "'");

                return RedirectToAction("Index", "User");
            } catch (Exception) { }
            return Redirect("~/User/Index");
        }

        public ActionResult ChangePassword(string id,
                                    string newPass)
        {
            try
            {
                DataModel db = new DataModel();
                db.get("EXEC ChangeUserPassword "
                        + "@UserId = " + id + ","
                        + "@NewPassword = '" + newPass + "'");

                return RedirectToAction("Index", "User");
            }
            catch (Exception) { }
            return Redirect("~/User/Index");
        }
    }
}