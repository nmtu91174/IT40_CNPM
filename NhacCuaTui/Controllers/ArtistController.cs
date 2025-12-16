using System;
using System.Collections;
using System.Web.Mvc;
using NhacCuaTui.Models;

namespace NhacCuaTui.Controllers
{
    public class ArtistController : Controller
    {
        // GET: Artist - Danh sách tất cả ca sĩ
        public ActionResult Index()
        {
            DataModel db = new DataModel();
            ArrayList artists = db.get("EXEC GetAllArtists;");
            ViewBag.Artists = artists;
            return View();
        }

        // GET: Artist/Detail/5 - Chi tiết và bài hát của ca sĩ
        public ActionResult Detail(int id)
        {
            DataModel db = new DataModel();
            ArrayList artist = db.get("EXEC GetArtistById @ArtistId = " + id + ";");
            ArrayList songs = db.get("SELECT s.song_id, s.song_name, s.release_date, s.file_name, " +
                "s.thumbnail_image, s.views_song, s.likes_count, al.album_name, g.genre_name " +
                "FROM Songs s " +
                "JOIN SongArtists sa ON s.song_id = sa.song_id " +
                "JOIN Artists a ON sa.artist_id = a.artist_id " +
                "LEFT JOIN Albums al ON s.album_id = al.album_id " +
                "LEFT JOIN Genres g ON s.genre_id = g.genre_id " +
                "WHERE a.artist_id = " + id + " ORDER BY s.views_song DESC;");

            ViewBag.Artist = artist;
            ViewBag.Songs = songs;
            return View();
        }
    }
}
