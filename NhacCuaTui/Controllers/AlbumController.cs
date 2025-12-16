using System;
using System.Collections;
using System.Web.Mvc;
using NhacCuaTui.Models;

namespace NhacCuaTui.Controllers
{
    public class AlbumController : Controller
    {
        private DataModel dataModel = new DataModel();

        // GET: Album/Index - Danh sách tất cả album
        public ActionResult Index(string searchAlbum = "", string filterGenre = "", string filterYear = "")
        {
            ArrayList albumList = dataModel.GetAllAlbums();
            
            // Fallback nếu stored procedure không hoạt động
            if (albumList == null || albumList.Count == 0)
            {
                albumList = dataModel.get("SELECT TOP 50 * FROM Albums ORDER BY ReleaseDate DESC");
            }
            
            if (!string.IsNullOrEmpty(searchAlbum))
            {
                ArrayList filtered = new ArrayList();
                foreach (ArrayList album in albumList)
                {
                    if (album.Count >= 2 && album[1].ToString().ToLower().Contains(searchAlbum.ToLower()))
                    {
                        filtered.Add(album);
                    }
                }
                albumList = filtered;
            }

            ViewBag.Albums = albumList ?? new ArrayList();
            ViewBag.SearchAlbum = searchAlbum;
            ViewBag.FilterGenre = filterGenre;
            ViewBag.FilterYear = filterYear;
            ViewBag.Genres = GetGenresList();
            
            return View();
        }

        // GET: Album/Detail/{id} - Chi tiết album
        public ActionResult Detail(int id)
        {
            ArrayList albumDetailList = dataModel.GetAlbumById(id);
            ArrayList albumSongs = dataModel.GetSongsInAlbum(id);

            if (albumDetailList == null || albumDetailList.Count == 0)
            {
                return HttpNotFound();
            }

            // Extract first row from result set
            ArrayList albumDetail = (ArrayList)albumDetailList[0];
            
            ViewBag.Album = albumDetail;
            ViewBag.Songs = albumSongs ?? new ArrayList();
            ViewBag.AlbumId = id;

            return View();
        }

        // AJAX: Add entire album to playlist
        [HttpPost]
        public JsonResult AddAlbumToPlaylist(int albumId, int playlistId)
        {
            try
            {
                if (Session["userId"] == null)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập" });
                }

                ArrayList songs = dataModel.GetSongsInAlbum(albumId);
                
                foreach (ArrayList song in songs)
                {
                    if (song.Count >= 1)
                    {
                        int songId = Convert.ToInt32(song[0]);
                        // Gọi hàm thêm bài hát vào playlist
                        dataModel.AddSongToPlaylist(songId, playlistId);
                    }
                }

                return Json(new { success = true, message = "Album đã được thêm vào playlist" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // Helper method - Get list of genres
        private ArrayList GetGenresList()
        {
            // Return common genres - adjust based on your database
            ArrayList genres = new ArrayList
            {
                "Nhạc Trẻ",
                "Trữ Tình",
                "Remix",
                "Rap",
                "Rock",
                "Pop",
                "Jazz",
                "K-Pop",
                "C-Pop"
            };
            return genres;
        }
    }
}
