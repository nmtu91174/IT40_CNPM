using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using NhacCuaTui.Models;

namespace NhacCuaTui.Areas.Admin.Controllers
{
    public class SongsController : Controller
    {
        DataModel db = new DataModel();
        // GET: Admin/Songs
        public ActionResult Index()
        {
            if (Session["Role"] == null)
            {
                return Redirect("~/Login/Login");
            }
            if (Session["Role"].Equals("Admin"))
            {
                ViewBag.listSongs = db.get("EXEC GetAllSongs");
                return View();
            }
            else
            {
                return Redirect("~/Home/Index");
            }
        }

        public ActionResult AddSong()
        {
            if (Session["Role"] == null)
            {
                return Redirect("~/Login/Login");
            }
            if (Session["Role"].Equals("Admin"))
            {
                ViewBag.listAlbums = db.get("EXEC GetAlbumWithArtists");
                ViewBag.listGenres = db.get("EXEC GetAllGenres");
                return View();
            }
            else
            {
                return Redirect("~/Home/Index");
            }
        }

        [HttpPost]
        public ActionResult ThemBaiHat(string songname,
                               int album_id,
                               int genre_id,
                               string lyrics,
                               DateTime release_date,
                               HttpPostedFileBase file,
                               HttpPostedFileBase thumbnail_image)
        {
            // Kiểm tra quyền Admin
            if (Session["Role"].Equals("Admin") == false)
            {
                return Redirect("~/Home/Index");
            }

            try
            {
                // Đường dẫn và xử lý file nhạc (lưu vào thư mục Source/mp3)
                string fileName = ""; // Khai báo bên ngoài để dùng trong escape
                string filePath = null;
                if (file != null && file.ContentLength > 0)
                {
                    fileName = Path.GetFileName(file.FileName);
                    string targetDir = Server.MapPath("~/Source/mp3");
                    // Tạo thư mục nếu chưa tồn tại để tránh lỗi
                    if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);
                    filePath = Path.Combine(targetDir, fileName);
                    file.SaveAs(filePath);
                }
                else
                {
                    Session["AddSong_Error"] = "File nhạc không được để trống.";
                    return RedirectToAction("AddSong", "Songs", "Admin");
                }

                // Đường dẫn và xử lý ảnh thumbnail (lưu vào thư mục Source/Musics-Image)
                string thumbnailName = ""; // Không có ảnh mặc định
                if (thumbnail_image != null && thumbnail_image.ContentLength > 0)
                {
                    thumbnailName = Path.GetFileName(thumbnail_image.FileName);
                    string thumbDir = Server.MapPath("~/Source/Musics-Image");
                    if (!Directory.Exists(thumbDir)) Directory.CreateDirectory(thumbDir);
                    string thumbnailFullPath = Path.Combine(thumbDir, thumbnailName);
                    thumbnail_image.SaveAs(thumbnailFullPath);
                }
                
                // Escape single quotes để tránh SQL injection
                string escapedSongName = (songname ?? "").Replace("'", "''");
                string escapedLyrics = (lyrics ?? "").Replace("'", "''");
                string escapedFileName = (fileName ?? "").Replace("'", "''");
                string escapedThumbnail = (thumbnailName ?? "").Replace("'", "''");
                
                db.get("EXEC AddSong " +
                        "@SongName = N'" + escapedSongName + "', " +
                        "@AlbumID = " + album_id + ", " +
                        "@GenreID = " + genre_id + ", " +
                        "@Lyrics = N'" + escapedLyrics + "', " +
                        "@ReleaseDate = '" + release_date.ToString("yyyy-MM-dd") + "', " +
                        "@FileName = N'" + escapedFileName + "', " +
                        "@ThumbnailImage = N'" + escapedThumbnail + "';");

                return RedirectToAction("Index", "Songs", "Admin");
            }
            catch (Exception)
            {
                Session["AddSong_Error"] = "Có lỗi xảy ra khi thêm bài hát.";
            }

            return RedirectToAction("AddSong", "Songs", "Admin");
        }



        public ActionResult EditSong(int id)
        {
            if (Session["Role"] == null)
            {
                return Redirect("~/Login/Login");
            }
            if (Session["Role"].Equals("Admin"))
            {
                ViewBag.GetSongById = db.get("EXEC GetSongById " + id + ";");
                ViewBag.listAlbums = db.get("EXEC GetAlbumWithArtists");
                ViewBag.listGenres = db.get("EXEC GetAllGenres");
                return View();
            }
            else
            {
                return Redirect("~/Login/Login");
            }
        }

        [HttpPost]
        public ActionResult SuaBaiHat(int song_id,
                              string songname,
                              int album_id,
                              int genre_id,
                              string lyrics,
                              DateTime release_date,
                              string file_name,
                              HttpPostedFileBase file,
                              string thumbnail_image,
                              HttpPostedFileBase thumbnail_image_file)
        {
            if (Session["Role"].Equals("Admin") == false)
            {
                return Redirect("~/Home/Index");
            }

            try
            {
                DataModel db = new DataModel();

                // Kiểm tra và xử lý file bài hát mới
                if (file != null && file.ContentLength > 0)
                {
                    string newFileName = Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Source/mp3"), newFileName);
                    file.SaveAs(filePath);
                    file_name = newFileName; // Cập nhật tên file mới
                }

                // Kiểm tra và xử lý ảnh thumbnail mới
                if (thumbnail_image_file != null && thumbnail_image_file.ContentLength > 0)
                {
                    string newThumbnailName = Path.GetFileName(thumbnail_image_file.FileName);
                    string thumbnailPath = Path.Combine(Server.MapPath("~/Source/Musics-Image"), newThumbnailName);
                    thumbnail_image_file.SaveAs(thumbnailPath);
                    thumbnail_image = newThumbnailName; // Cập nhật tên ảnh mới
                }

                // Escape single quotes để tránh SQL injection
                string escapedLyrics = (lyrics ?? "").Replace("'", "''");
                string escapedSongName = (songname ?? "").Replace("'", "''");
                string escapedFileName = (file_name ?? "").Replace("'", "''");
                string escapedThumbnail = (thumbnail_image ?? "").Replace("'", "''");
                
                // Gọi Stored Procedure để cập nhật bài hát
                string sql = "EXEC UpdateSongById " +
                       "@SongId = " + song_id + ", " +
                       "@SongName = N'" + escapedSongName + "', " +
                       "@AlbumID = " + album_id + ", " +
                       "@GenreID = " + genre_id + ", " +
                       "@Lyrics = N'" + escapedLyrics + "', " +
                       "@ReleaseDate = '" + release_date.ToString("yyyy-MM-dd") + "', " +
                       "@FileName = N'" + escapedFileName + "', " +
                       "@ThumbnailImage = N'" + escapedThumbnail + "';";
                
                db.get(sql);

                return RedirectToAction("Index", "Songs", "Admin");
            }
            catch (Exception ex)
            {
                Session["EditSong_Error"] = "Có lỗi xảy ra khi sửa bài hát: " + ex.Message;
            }

            return RedirectToAction("EditSong", "Songs", "Admin");
        }


        public ActionResult XoaBaiHat(int id)
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
                DataModel db = new DataModel();
                db.get("EXEC DeleteSong @SongId = " + id + ";");
                return RedirectToAction("Index", "Songs", "Admin");
            }
            catch (Exception)
            {
                Session["DeleteSong_Error"] = "Có lỗi xảy ra khi xóa bài hát.";
                return RedirectToAction("Index", "Songs", "Admin");
            }
        }

    }
}