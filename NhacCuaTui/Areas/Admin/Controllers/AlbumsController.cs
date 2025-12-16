using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using NhacCuaTui.Models;
using System.Collections;

namespace NhacCuaTui.Areas.Admin.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly DataModel db = new DataModel();

        // ================== CHECK ROLE ADMIN ==================
        private bool IsAdmin()
        {
            if (Session["Role"] == null) return false;
            return Session["Role"].ToString().Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }

        private ActionResult RedirectIfNotAdmin()
        {
            if (Session["Role"] == null)
                return Redirect("~/Login/Login");

            if (!IsAdmin())
                return Redirect("~/Home/Index");

            return null; // OK
        }

        // GET: Admin/Albums
        public ActionResult Index()
        {
            if (Session["Role"] == null)
            {
                return Redirect("~/Login/Login");
            }
            if (Session["Role"].Equals("Admin"))
            {
                DataModel db = new DataModel();
                ViewBag.listAlbums = db.get("EXEC GetAlbumWithArtists");
                return View();
            }
            else
            {
                return Redirect("~/Home/Index");
            }
        }

        // ================== THÊM ALBUM (GET) ==================
        [HttpGet]
        public ActionResult AddAlbum()
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null) return redirect;

            // Danh sách nghệ sĩ
            ViewBag.Artists = db.get("SELECT artist_id, artist_name FROM Artists ORDER BY artist_name;");

            // Danh sách bài hát để gán vào album
            ViewBag.Songs = db.get("SELECT song_id, song_name FROM Songs ORDER BY song_name;");

            return View();
        }

        // ================== THÊM ALBUM (POST) ==================
        [HttpPost]
        public ActionResult ThemAlbum(string albumname, int artist_id, DateTime release_date,
                                      HttpPostedFileBase cover_image, List<int> existing_songs)
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null) return redirect;

            try
            {
                // ----- Lưu ảnh bìa -----
                string coverImageName = "default-cover.jpg";
                if (cover_image != null && cover_image.ContentLength > 0)
                {
                    coverImageName = Path.GetFileName(cover_image.FileName);
                    string path = Path.Combine(Server.MapPath("~/Source/Albums-Image"), coverImageName);
                    Directory.CreateDirectory(Path.GetDirectoryName(path)); // đảm bảo tồn tại thư mục
                    cover_image.SaveAs(path);
                }

                // Escape ' trong tên album
                string safeAlbumName = (albumname ?? "").Replace("'", "''");
                string safeCover = (coverImageName ?? "").Replace("'", "''");

                // ----- Gọi proc AddAlbum -----
                string sqlAdd = string.Format(@"
                    EXEC AddAlbum 
                        @AlbumName = N'{0}',
                        @ArtistID = {1},
                        @ReleaseDate = '{2}',
                        @CoverImage = N'{3}';
                ",
                    safeAlbumName,
                    artist_id,
                    release_date.ToString("yyyy-MM-dd"),
                    safeCover
                );

                db.get(sqlAdd);

                // ----- Lấy AlbumId mới nhất sau khi thêm -----
                ArrayList albumIdRows = db.get("SELECT TOP 1 album_id FROM Albums ORDER BY album_id DESC;");
                if (albumIdRows.Count == 0)
                {
                    TempData["Error"] = "Không lấy được ID album vừa thêm.";
                    return RedirectToAction("AddAlbum");
                }

                int albumId = int.Parse(((ArrayList)albumIdRows[0])[0].ToString());

                // ----- Gán các bài hát đã chọn vào album -----
                if (existing_songs != null && existing_songs.Count > 0)
                {
                    foreach (int songId in existing_songs)
                    {
                        string sqlAddSong = string.Format(@"
                            EXEC AddSongToAlbum 
                                @AlbumId = {0},
                                @SongId = {1};
                        ", albumId, songId);

                        db.get(sqlAddSong);
                    }
                }

                TempData["Success"] = "Thêm album thành công.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi thêm album: " + ex.Message;
                return RedirectToAction("AddAlbum");
            }
        }

        // ================== SỬA ALBUM (GET) ==================
        [HttpGet]
        public ActionResult EditAlbum(int id)
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null) return redirect;

            // Thông tin album
            ArrayList albumRows = db.get("EXEC GetAlbumById @AlbumId = " + id + ";");
            if (albumRows.Count == 0)
            {
                return HttpNotFound("Không tìm thấy album.");
            }

            ViewBag.Album = (ArrayList)albumRows[0];

            // Danh sách nghệ sĩ
            ViewBag.Artists = db.get("SELECT artist_id, artist_name FROM Artists ORDER BY artist_name;");

            // Tất cả bài hát
            ViewBag.AllSongs = db.get("SELECT song_id, song_name FROM Songs ORDER BY song_name;");

            // Các bài hát hiện đang thuộc album này
            ViewBag.SongsInAlbum = db.get("EXEC GetSongsInAlbum @AlbumId = " + id + ";");

            return View();
        }

        // ================== SỬA ALBUM (POST) ==================
        [HttpPost]
        public ActionResult SuaAlbum(int id, string albumname, int artist_id, DateTime release_date,
                                     HttpPostedFileBase cover_image, List<int> existing_songs)
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null) return redirect;

            try
            {
                // Lấy thông tin album hiện tại (để giữ cover cũ nếu không upload mới)
                ArrayList albumRows = db.get("EXEC GetAlbumById @AlbumId = " + id + ";");
                if (albumRows.Count == 0)
                {
                    TempData["Error"] = "Không tìm thấy album để sửa.";
                    return RedirectToAction("Index");
                }

                ArrayList album = (ArrayList)albumRows[0];

                string currentCover = album[5] != null ? album[5].ToString() : "default-cover.jpg";
                string coverImageName = currentCover;

                // ----- Nếu có upload ảnh bìa mới -----
                if (cover_image != null && cover_image.ContentLength > 0)
                {
                    coverImageName = Path.GetFileName(cover_image.FileName);
                    string path = Path.Combine(Server.MapPath("~/Source/Albums-Image"), coverImageName);
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    cover_image.SaveAs(path);
                }

                string safeAlbumName = (albumname ?? "").Replace("'", "''");
                string safeCover = (coverImageName ?? "").Replace("'", "''");

                // ----- Cập nhật thông tin album -----
                string sqlUpdate = string.Format(@"
                    EXEC UpdateAlbum
                        @AlbumId = {0},
                        @AlbumName = N'{1}',
                        @ArtistID = {2},
                        @ReleaseDate = '{3}',
                        @CoverImage = N'{4}';
                ",
                    id,
                    safeAlbumName,
                    artist_id,
                    release_date.ToString("yyyy-MM-dd"),
                    safeCover
                );

                db.get(sqlUpdate);

                // ----- Bỏ hết bài hát khỏi album (set album_id = NULL) -----
                db.get("EXEC RemoveAllSongsFromAlbum @AlbumId = " + id + ";");

                // ----- Gán lại các bài hát đang được chọn -----
                if (existing_songs != null && existing_songs.Count > 0)
                {
                    foreach (int songId in existing_songs)
                    {
                        string sqlAddSong = string.Format(@"
                            EXEC AddSongToAlbum 
                                @AlbumId = {0},
                                @SongId = {1};
                        ", id, songId);

                        db.get(sqlAddSong);
                    }
                }

                TempData["Success"] = "Cập nhật album thành công.";
                return RedirectToAction("EditAlbum", new { id = id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi sửa album: " + ex.Message;
                return RedirectToAction("EditAlbum", new { id = id });
            }
        }


        // ================== XÓA ALBUM ==================
        [HttpPost]
        public ActionResult DeleteAlbum(int id)
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null) return redirect;

            try
            {
                db.get("EXEC DeleteAlbum @AlbumId = " + id + ";");
                TempData["Success"] = "Xóa album thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa album: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // ================== XÓA ALBUM (Vietnamese route) ==================
        // GET: show confirmation
        [HttpGet]
        public ActionResult XoaAlbum(int id)
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null) return redirect;

            // Lấy thông tin album để hiển thị xác nhận
            ArrayList albumRows = db.get("EXEC GetAlbumById @AlbumId = " + id + ";");
            if (albumRows.Count == 0)
            {
                return HttpNotFound("Không tìm thấy album.");
            }

            ViewBag.Album = (ArrayList)albumRows[0];
            return View();
        }

        // POST: thực hiện xóa (action name remains XoaAlbum so form can post to the same URL)
        [HttpPost, ActionName("XoaAlbum")]
        [ValidateAntiForgeryToken]
        public ActionResult XoaAlbumPost(int id)
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null) return redirect;

            try
            {
                db.get("EXEC DeleteAlbum @AlbumId = " + id + ";");
                TempData["Success"] = "Xóa album thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa album: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

    }
}