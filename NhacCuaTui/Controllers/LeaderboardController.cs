using System;
using System.Collections;
using System.Web.Mvc;
using NhacCuaTui.Models;

namespace NhacCuaTui.Controllers
{
    public class LeaderboardController : Controller
    {
        // GET: Leaderboard
        public ActionResult Index()
        {
            DataModel db = new DataModel();
            ArrayList topSongs = db.get("EXEC GetTop10SongsByViews;");
            ViewBag.TopSongs = topSongs;
            return View();
        }
    }
}
