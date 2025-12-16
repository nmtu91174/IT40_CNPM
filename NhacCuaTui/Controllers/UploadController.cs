using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using NhacCuaTui.Models;

namespace NhacCuaTui.Controllers
{
    public class UploadController : Controller
    {
        private DataModel db = new DataModel();

        [HttpPost]
        public ActionResult UploadSong()
        {
            // Check if user is logged in
            if (Session["userID"] == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để tải lên bài hát!" });
            }

            try
            {
                if (Request.Files.Count == 0)
                {
                    return Json(new { success = false, message = "Vui lòng chọn file nhạc!" });
                }

                var songFile = Request.Files["songFile"];
                var thumbnailFile = Request.Files.Count > 1 ? Request.Files["thumbnailFile"] : null;

                // Get form data
                string songName = Request.Form["songName"];
                string artistName = Request.Form["artistName"];
                string genre = Request.Form["genre"];
                string description = Request.Form["description"];
                int userID = Convert.ToInt32(Session["userID"]);

                // Validate input
                if (string.IsNullOrWhiteSpace(songName) || string.IsNullOrWhiteSpace(artistName) || string.IsNullOrWhiteSpace(genre))
                {
                    return Json(new { success = false, message = "Vui lòng điền đầy đủ thông tin!" });
                }

                // Validate song file
                if (songFile == null || songFile.ContentLength == 0)
                {
                    return Json(new { success = false, message = "File nhạc không hợp lệ!" });
                }

                // Check file size (50MB max)
                const int maxFileSize = 50 * 1024 * 1024;
                if (songFile.ContentLength > maxFileSize)
                {
                    return Json(new { success = false, message = "Kích thước file vượt quá 50MB!" });
                }

                // Check file type
                string[] allowedExtensions = { ".mp3" };
                string fileExtension = Path.GetExtension(songFile.FileName).ToLower();
                if (Array.IndexOf(allowedExtensions, fileExtension) == -1)
                {
                    return Json(new { success = false, message = "Chỉ chấp nhận file MP3!" });
                }

                // Create directories if they don't exist
                string uploadsPath = Server.MapPath("~/Source/mp3");
                string imageUploadPath = Server.MapPath("~/Source/Musics-Image");

                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }
                if (!Directory.Exists(imageUploadPath))
                {
                    Directory.CreateDirectory(imageUploadPath);
                }

                // Save song file
                string fileName = Path.GetFileNameWithoutExtension(songFile.FileName);
                string uniqueFileName = Path.Combine(uploadsPath, DateTime.Now.Ticks + "_" + songFile.FileName);
                string uniqueFileNameOnly = DateTime.Now.Ticks + "_" + songFile.FileName;
                songFile.SaveAs(uniqueFileName);

                // Save thumbnail if provided
                string thumbnailFileName = "default.png";
                if (thumbnailFile != null && thumbnailFile.ContentLength > 0)
                {
                    try
                    {
                        // Check thumbnail file size (5MB max)
                        const int maxThumbnailSize = 5 * 1024 * 1024;
                        if (thumbnailFile.ContentLength <= maxThumbnailSize)
                        {
                            string[] allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                            string imageExtension = Path.GetExtension(thumbnailFile.FileName).ToLower();

                            if (Array.IndexOf(allowedImageExtensions, imageExtension) == -1)
                            {
                                return Json(new { success = false, message = "Định dạng ảnh không hợp lệ!" });
                            }

                            thumbnailFileName = DateTime.Now.Ticks + "_" + thumbnailFile.FileName;
                            string thumbnailPath = Path.Combine(imageUploadPath, thumbnailFileName);
                            thumbnailFile.SaveAs(thumbnailPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Use default image if thumbnail upload fails
                        System.Diagnostics.Debug.WriteLine("Thumbnail upload failed: " + ex.Message);
                    }
                }

                // Save to database
                try
                {
                    // Get genre_id from genre_name
                    string genreCheckSql = $"SELECT genre_id FROM Genres WHERE genre_name = N'{genre.Replace("'", "''")}'";
                    object genreIdObj = db.getScalar(genreCheckSql);
                    int genreId = 0;
                    
                    if (genreIdObj != null)
                    {
                        genreId = Convert.ToInt32(genreIdObj);
                    }

                    // Insert artist if not exists
                    string artistCheckSql = $"SELECT artist_id FROM Artists WHERE artist_name = N'{artistName.Replace("'", "''")}'";
                    object artistIdObj = db.getScalar(artistCheckSql);
                    int artistId;

                    if (artistIdObj == null)
                    {
                        // Insert new artist
                        string insertArtistSql = $"INSERT INTO Artists (artist_name, created_at, updated_at) VALUES (N'{artistName.Replace("'", "''")}', GETDATE(), GETDATE())";
                        db.ExecuteNonQuery(insertArtistSql);
                        artistIdObj = db.getScalar(artistCheckSql);
                    }

                    artistId = Convert.ToInt32(artistIdObj);

                    // Insert new song
                    string genreIdValue = genreId > 0 ? genreId.ToString() : (string)"NULL";
                    string insertSongSql = $@"INSERT INTO Songs 
                        (song_name, album_id, genre_id, file_name, thumbnail_image, views_song, likes_count, release_date, created_at, updated_at) 
                        VALUES 
                        (N'{songName.Replace("'", "''")}', NULL, {genreIdValue}, N'{uniqueFileNameOnly}', N'{thumbnailFileName}', 0, 0, GETDATE(), GETDATE(), GETDATE())";
                    
                    db.ExecuteNonQuery(insertSongSql);

                    // Get the last inserted song ID
                    string getLastSongIdSql = "SELECT IDENT_CURRENT('Songs')";
                    object lastSongIdObj = db.getScalar(getLastSongIdSql);
                    
                    if (lastSongIdObj != null && lastSongIdObj != DBNull.Value)
                    {
                        int songId = Convert.ToInt32(Convert.ToDecimal(lastSongIdObj));

                        // Insert into SongArtists table
                        string insertSongArtistSql = $"INSERT INTO SongArtists (song_id, artist_id) VALUES ({songId}, {artistId})";
                        db.ExecuteNonQuery(insertSongArtistSql);
                    }

                    return Json(new { success = true, message = "Tải lên bài hát thành công!" });
                }
                catch (Exception dbEx)
                {
                    System.Diagnostics.Debug.WriteLine("Database error: " + dbEx.Message);
                    System.Diagnostics.Debug.WriteLine("Stack trace: " + dbEx.StackTrace);
                    return Json(new { success = false, message = "Lỗi lưu vào database: " + dbEx.Message });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Upload error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

    }
}
