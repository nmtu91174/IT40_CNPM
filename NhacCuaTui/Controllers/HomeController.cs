using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NhacCuaTui.Models;

namespace NhacCuaTui.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            DataModel db = new DataModel();
            //Slide
            ViewBag.list12BaiHatNoiBat = db.get("EXEC GetTop12SongsByViews;");
            //List bai hat
            ViewBag.list10BaiHatMoiNhat = db.get("EXEC GetTop10NewestSongs;");
            //List albums
            ViewBag.list10AlBums = db.get("EXEC GetTop10Albums;");
            //Nghe nhieu
            ViewBag.list10BaiNgheNhieu = db.get("EXEC GetTop10SongsToday;");
            //BXH
            ViewBag.Top10BXH = db.get("EXEC GetTop10SongsByViews;");
            //Tim kiem
            ViewBag.GetAllSong = db.get("EXEC GetAllSongs;");
            //Ca sĩ nhiều like nhất
            ViewBag.Top6CaSi = db.get("EXEC GetTop6ArtistsWithRanking;");
            //Featured Artists
            ViewBag.FeaturedArtists = db.get("EXEC GetAllArtists;");
            return View();
        }

        public ActionResult TheLoai(string Genres = "Nhạc Trẻ")
        {
            DataModel db = new DataModel();
            //Lấy danh sách tất cả bài hát
            ViewBag.listAllSongs = db.get("EXEC GetSongsByGenreName N'"+Genres+"';");
            //Lấy tên thể loại
            ViewBag.Genres = Genres;
            //BXH
            ViewBag.Top10BXH = db.get("EXEC GetTop10SongsByViews;");
            //Ca sĩ nhiều like nhất
            ViewBag.Top6CaSi = db.get("EXEC GetTop6ArtistsWithRanking;");
            //Tim kiem
            ViewBag.GetAllSong = db.get("EXEC GetAllSongs;");
            return View();
        }

        public ActionResult Search(string searchInput = " ")
        {
            DataModel db = new DataModel();
            //Lấy danh sách tất cả bài hát có từ tìm kiếm gần giống
            ViewBag.listAllSongs = db.get("EXEC SearchSongsOrArtists N'" + searchInput + "';");
            //Giá trị tìm kiếm
            ViewBag.search = searchInput;
            //BXH
            ViewBag.Top10BXH = db.get("EXEC GetTop10SongsByViews;");
            //Ca sĩ nhiều like nhất
            ViewBag.Top6CaSi = db.get("EXEC GetTop6ArtistsWithRanking;");
            //Tim kiem
            ViewBag.GetAllSong = db.get("EXEC GetAllSongs;");
            return View();
        }

        public ActionResult Error404NotFound()
        {
            return View();
        }
    }
}