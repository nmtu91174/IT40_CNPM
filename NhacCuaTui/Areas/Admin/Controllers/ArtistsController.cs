using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using NhacCuaTui.Models;

namespace NhacCuaTui.Areas.Admin.Controllers
{
    public class ArtistsController : Controller
    {
        // GET: Admin/Artists
        public ActionResult Index()
        {
            if (Session["Role"] == null)
            {
                return Redirect("~/Login/Login");
            }
            if (Session["Role"].Equals("Admin"))
            {
                DataModel db = new DataModel();
                ViewBag.listArtists = db.get("exec GetAllArtists");
                return View();
            }
            else
            {
                return Redirect("~/Home/Index");
            }
        }

        public ActionResult AddArtist()
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
        public ActionResult ThemCaSi(string artistname,
                                    string bio,
                                    HttpPostedFileBase avatar_image)
        {
            if (Session["Role"].Equals("Admin") == false)
            {
                return Redirect("~/Home/Index");
            }
            try
            {
                if (avatar_image != null && avatar_image.ContentLength > 0)
                {
                    string filename = Path.GetFileName(avatar_image.FileName);
                    string path = Path.Combine(Server.MapPath("~/Source/Artists-Image"), filename);
                    avatar_image.SaveAs(path);
                    DataModel db = new DataModel();
                    db.get("EXEC AddArtist " +
                            "@ArtistName = N'" + artistname + "', " +
                            "@Bio = N'" + bio + "', " +
                            "@AvatarImage = N'" + avatar_image.FileName + "'; ");
                }
                else
                {                    
                    DataModel db = new DataModel();
                    string image_default = "default.jpg";
                    db.get("EXEC AddArtist " +
                            "@ArtistName = N'" + artistname + "', " +
                            "@Bio = N'" + bio + "', " +
                            "@AvatarImage = N'" + image_default + "'; ");
                }
                return RedirectToAction("Index", "Artists", "Admin");
            }
            catch (Exception) { }
            Session["AddArtist_Error"] = "Đã tồn tại ca sĩ này hoặc thêm hình ảnh ca sĩ";
            return RedirectToAction("AddArtist", "Artists", "Admin");
        }

        public ActionResult EditArtist(int id)
        {
            if (Session["Role"] == null)
            {
                return Redirect("~/Login/Login");
            }
            if (Session["Role"].Equals("Admin"))
            {
                DataModel db = new DataModel();
                ViewBag.GetArtistById = db.get("EXEC GetArtistById " + id + ";");
                return View();
            }
            else
            {
                return Redirect("~/Login/Login");
            }
        }

        [HttpPost]
        public ActionResult SuaCaSi(int id,
                                    string artistname,
                                    string bio,
                                    string avatar_image,
                                    HttpPostedFileBase avatar_image2)
        {
            if (Session["Role"].Equals("Admin") == false)
            {
                return Redirect("~/Home/Index");
            }
            try
            {
                DataModel db = new DataModel();

                if (avatar_image2 != null && avatar_image2.ContentLength > 0)
                {
                    string filename = Path.GetFileName(avatar_image2.FileName);
                    string path = Path.Combine(Server.MapPath("~/Source/Artists-Image"), filename);
                    avatar_image2.SaveAs(path);
                    db.get("EXEC UpdateArtistById " +
                            "@ArtistId = "+ id +"," +
                            "@ArtistName = N'" + artistname + "', " +
                            "@Bio = N'" + bio + "', " +
                            "@AvatarImage = N'" + avatar_image2.FileName + "'; ");
                }
                else
                {
                    db.get("EXEC UpdateArtistById " +
                            "@ArtistId = " + id + "," +
                            "@ArtistName = N'" + artistname + "', " +
                            "@Bio = N'" + bio + "', " +
                            "@AvatarImage = N'" + avatar_image + "'; ");
                }
                return RedirectToAction("Index", "Artists", "Admin");
            }
            catch (Exception) { }
            Session["AddUser_Error"] = "Đã tồn tại ca sĩ";
            return RedirectToAction("EditArtist", "Artists", "Admin");
        }

        public ActionResult XoaCaSi(string id)
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
                db.get("EXEC DeleteArtist @ArtistId = " + id + ";");
                return RedirectToAction("Index", "Artists", "Admin");
            }
            else
            {
                return Redirect("~/Home/Index");
            }
        }
    }
}