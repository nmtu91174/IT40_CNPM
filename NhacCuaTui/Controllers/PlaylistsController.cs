using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NhacCuaTui.Models;

namespace NhacCuaTui.Controllers
{
    public class PlaylistsController : Controller
    {
        DataModel db = new DataModel();
        public ActionResult Index(int? playlistId = null)
        {
            if (playlistId.HasValue)
            {
                ViewBag.SelectedPlaylistId = playlistId.Value;
            }
            return View();
        }

        [HttpPost]
        public ActionResult ThemPlaylist(string playlist_name, List<int> selected_songs)
        {
            if (Session["UserId"] == null)
            {
                return Redirect("~/Login");
            }

            try
            {
                int userId = (int)Session["UserId"];
                DataModel db = new DataModel();

                // Thêm Playlist mới
                int playlistId = (int)db.getScalar($"EXEC AddPlaylist @UserId = {userId}, @PlaylistName = N'{playlist_name}'");

                // Thêm bài hát vào Playlist
                if (selected_songs != null)
                {
                    foreach (int songId in selected_songs)
                    {
                        db.get($"EXEC AddSongToPlaylist @PlaylistId = {playlistId}, @SongId = {songId}");
                    }
                }

                TempData["Success"] = "Playlist đã được tạo thành công!";
                return RedirectToAction("Index", "Playlists");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("ThemPlaylist", "Playlists");
            }
        }

        [HttpGet]
        public JsonResult GetUserPlaylists(int songId)
        {
            if (Session["userId"] == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để quản lý playlist." }, JsonRequestBehavior.AllowGet);
            }

            int userId = int.Parse("" + Session["userId"]);
            DataModel db = new DataModel();

            // Lấy danh sách playlist từ cơ sở dữ liệu
            var playlists = db.get($"SELECT playlist_id, playlist_name FROM Playlists WHERE user_id = {userId}");

            // Chuyển đổi ArrayList sang danh sách JSON trả về
            var result = new List<object>();
            foreach (ArrayList playlist in playlists)
            {
                result.Add(new
                {
                    id = playlist[0],   // playlist_id
                    name = playlist[1]  // playlist_name
                });
            }

            return Json(new { success = true, playlists = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddSongToPlaylist(int songId, int playlistId)
        {
            try
            {
                DataModel db = new DataModel();
                db.get($"EXEC AddSongToPlaylist @PlaylistId = {playlistId}, @SongId = {songId}");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        [HttpPost]
        public JsonResult CreateNewPlaylist(string name)
        {
            try
            {
                int userId = int.Parse(""+Session["userId"]);
                DataModel db = new DataModel();
                db.get($"EXEC AddPlaylist @UserId = {userId}, @PlaylistName = N'{name}'");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult RemoveSong(int playlistId, int songId)
        {
            try
            {
                DataModel db = new DataModel();
                db.getAPI($"EXEC RemoveSongFromPlaylist @PlaylistId = {playlistId}, @SongId = {songId}");
                return Json(new { success = true, message = "Bài hát đã được xóa khỏi playlist." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetSongsInPlaylist(int playlistId)
        {
            try
            {
                if (Session["UserId"] == null)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để xem bài hát." }, JsonRequestBehavior.AllowGet);
                }

                var songs = db.get($"EXEC GetSongsInPlaylist @PlaylistId = {playlistId}");

                var result = songs.Cast<ArrayList>().Select(song => new
                {
                    SongId = song[0],
                    SongName = song[1],
                    ReleaseDate = song[2],
                    FileName = song[3],
                    ThumbnailImage = song[4],
                    ViewsSong = song[5],
                    LikesCount = song[6],
                    AlbumName = song[7],
                    GenreName = song[8],
                    ArtistName = song[9]
                }).ToList();

                return Json(new { success = true, songs = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult AddPlaylist(string name)
        {
            try
            {
                int userId = (int)Session["UserId"];
                db.getAPI($"EXEC AddPlaylist @UserId = {userId}, @PlaylistName = N'{name}'");
                return Json(new { success = true, message = "Playlist đã được thêm thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeletePlaylist(int playlistId)
        {
            try
            {
                db.getAPI($"EXEC DeletePlaylist @PlaylistId = {playlistId}");
                return Json(new { success = true, message = "Playlist đã được xóa thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

    }
}