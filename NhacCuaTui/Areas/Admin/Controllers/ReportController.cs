using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NhacCuaTui.Models;
using System.Collections;
using Newtonsoft.Json;

namespace NhacCuaTui.Areas.Admin.Controllers
{
    public class ReportController : Controller
    {
        private DataModel db = new DataModel();

        // API endpoint - Top 10 songs by play count
        public JsonResult GetTopSongsByPlays()
        {
            ArrayList data = db.get(@"
                SELECT TOP 10 
                    song_name as name, 
                    views_song as plays 
                FROM Songs 
                ORDER BY views_song DESC
            ");

            List<object> result = new List<object>();
            foreach (ArrayList row in data)
            {
                int plays = 0;
                if (row[1] != null && !string.IsNullOrEmpty(row[1].ToString()))
                    int.TryParse(row[1].ToString(), out plays);
                result.Add(new { name = row[0], plays = plays });
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // API endpoint - Top 10 artists by like count
        public JsonResult GetTopArtistsByLikes()
        {
            ArrayList data = db.get(@"
                SELECT TOP 10 
                    artist_name as name, 
                    COUNT(sa.song_id) as likes 
                FROM Artists a
                LEFT JOIN SongArtists sa ON a.artist_id = sa.artist_id
                GROUP BY artist_name
                ORDER BY likes DESC
            ");

            List<object> result = new List<object>();
            foreach (ArrayList row in data)
            {
                int likes = 0;
                if (row[1] != null && !string.IsNullOrEmpty(row[1].ToString()))
                    int.TryParse(row[1].ToString(), out likes);
                result.Add(new { name = row[0], likes = likes });
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // API endpoint - Songs by genre
        public JsonResult GetSongsByGenre()
        {
            ArrayList data = db.get(@"
                SELECT 
                    genre_name as name, 
                    COUNT(s.song_id) as count 
                FROM Genres g
                LEFT JOIN Songs s ON g.genre_id = s.genre_id
                GROUP BY genre_name
                ORDER BY count DESC
            ");

            List<object> result = new List<object>();
            foreach (ArrayList row in data)
            {
                int count = 0;
                if (row[1] != null && !string.IsNullOrEmpty(row[1].ToString()))
                    int.TryParse(row[1].ToString(), out count);
                result.Add(new { name = row[0], count = count });
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // API endpoint - Total statistics
        public JsonResult GetTotalStatistics()
        {
            object totalSongs = db.getScalar("SELECT COUNT(*) FROM Songs");
            object totalArtists = db.getScalar("SELECT COUNT(*) FROM Artists");
            object totalAlbums = db.getScalar("SELECT COUNT(*) FROM Albums");
            object totalUsers = db.getScalar("SELECT COUNT(*) FROM Users");
            object totalPlays = db.getScalar("SELECT SUM(views_song) FROM Songs");

            int tS = 0, tA = 0, tAl = 0, tU = 0, tP = 0;
            if (totalSongs != null) int.TryParse(totalSongs.ToString(), out tS);
            if (totalArtists != null) int.TryParse(totalArtists.ToString(), out tA);
            if (totalAlbums != null) int.TryParse(totalAlbums.ToString(), out tAl);
            if (totalUsers != null) int.TryParse(totalUsers.ToString(), out tU);
            if (totalPlays != null) int.TryParse(totalPlays.ToString(), out tP);

            return Json(new
            {
                totalSongs = tS,
                totalArtists = tA,
                totalAlbums = tAl,
                totalUsers = tU,
                totalPlays = tP
            }, JsonRequestBehavior.AllowGet);
        }

        // API endpoint - Top 10 albums
        public JsonResult GetTopAlbumsByViews()
        {
            ArrayList data = db.get(@"
                SELECT TOP 10 
                    album_name as name, 
                    COUNT(s.song_id) as songCount 
                FROM Albums a
                LEFT JOIN Songs s ON a.album_id = s.album_id
                GROUP BY album_name
                ORDER BY songCount DESC
            ");

            List<object> result = new List<object>();
            foreach (ArrayList row in data)
            {
                int songs = 0;
                if (row[1] != null && !string.IsNullOrEmpty(row[1].ToString()))
                    int.TryParse(row[1].ToString(), out songs);
                result.Add(new { name = row[0], songs = songs });
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
