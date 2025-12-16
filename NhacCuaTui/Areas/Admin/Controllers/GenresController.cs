using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NhacCuaTui.Models;

namespace NhacCuaTui.Areas.Admin.Controllers
{
    public class GenresController : Controller
    {
        private DataModel db = new DataModel();

        // GET: Admin/Genres
        public ActionResult Index()
        {
            if (Session["Role"] == null)
            {
                return Redirect("~/Login/Login");
            }
            if (Session["Role"].Equals("Admin"))
            {
                ViewBag.listGenres = db.get("EXEC GetAllGenres;");
                return View();
            }
            else
            {
                return Redirect("~/Home/Index");
            }
        }

        // GET: Admin/Genres/Create
        public ActionResult Create()
        {
            if (Session["Role"] == null)
            {
                return Redirect("~/Login/Login");
            }
            if (!Session["Role"].Equals("Admin"))
            {
                return Redirect("~/Home/Index");
            }
            return View();
        }

        // POST: Admin/Genres/Create
        [HttpPost]
        public ActionResult Create(string genreName, string region)
        {
            if (Session["Role"] == null)
            {
                return Redirect("~/Login/Login");
            }
            if (!Session["Role"].Equals("Admin"))
            {
                return Redirect("~/Home/Index");
            }

            try
            {
                if (string.IsNullOrWhiteSpace(genreName) || string.IsNullOrWhiteSpace(region))
                {
                    ViewBag.Error = "Vui lòng điền đầy đủ thông tin";
                    return View();
                }

                bool result = db.ExecuteNonQuery($"INSERT INTO Genres (genre_name, region) VALUES (N'{genreName}', N'{region}')");
                if (result)
                {
                    TempData["Success"] = "Thêm thể loại thành công";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "Lỗi khi thêm thể loại";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Có lỗi xảy ra: " + ex.Message;
                return View();
            }
        }

        // GET: Admin/Genres/Edit/5
        public ActionResult Edit(int id)
        {
            if (Session["Role"] == null)
            {
                return Redirect("~/Login/Login");
            }
            if (!Session["Role"].Equals("Admin"))
            {
                return Redirect("~/Home/Index");
            }

            var genres = db.get($"SELECT genre_id, genre_name, region FROM Genres WHERE genre_id = {id}");
            if (genres.Count == 0)
            {
                return HttpNotFound();
            }

            ArrayList genreData = (ArrayList)genres[0];
            ViewBag.GenreId = id;
            ViewBag.GenreName = genreData[1];
            ViewBag.Region = genreData[2];
            return View();
        }

        // POST: Admin/Genres/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, string genreName, string region)
        {
            if (Session["Role"] == null)
            {
                return Redirect("~/Login/Login");
            }
            if (!Session["Role"].Equals("Admin"))
            {
                return Redirect("~/Home/Index");
            }

            try
            {
                if (string.IsNullOrWhiteSpace(genreName) || string.IsNullOrWhiteSpace(region))
                {
                    ViewBag.Error = "Vui lòng điền đầy đủ thông tin";
                    ViewBag.GenreId = id;
                    ViewBag.GenreName = genreName;
                    ViewBag.Region = region;
                    return View();
                }

                bool result = db.ExecuteNonQuery($"UPDATE Genres SET genre_name = N'{genreName}', region = N'{region}' WHERE genre_id = {id}");
                if (result)
                {
                    TempData["Success"] = "Cập nhật thể loại thành công";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "Lỗi khi cập nhật thể loại";
                    ViewBag.GenreId = id;
                    ViewBag.GenreName = genreName;
                    ViewBag.Region = region;
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Có lỗi xảy ra: " + ex.Message;
                ViewBag.GenreId = id;
                ViewBag.GenreName = genreName;
                ViewBag.Region = region;
                return View();
            }
        }

        // GET: Admin/Genres/Delete/5
        public ActionResult Delete(int id)
        {
            if (Session["Role"] == null)
            {
                return Redirect("~/Login/Login");
            }
            if (!Session["Role"].Equals("Admin"))
            {
                return Redirect("~/Home/Index");
            }

            try
            {
                bool result = db.ExecuteNonQuery($"DELETE FROM Genres WHERE genre_id = {id}");
                if (result)
                {
                    TempData["Success"] = "Xóa thể loại thành công";
                }
                else
                {
                    TempData["Error"] = "Lỗi khi xóa thể loại";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}