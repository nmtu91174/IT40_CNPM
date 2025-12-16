using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Web.Mvc;
using NhacCuaTui.Models;

namespace NhacCuaTui.Areas.API.Controllers
{
    public class MyAPIController : Controller
    {
        DataModel db = new DataModel();

        public JsonResult GetAllData()
        {
            string sqlQuery = "EXEC GetAllData";
            Dictionary<string, ArrayList> result = db.getAllAPI(sqlQuery);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllSongs()
        {
            ArrayList a = db.getAPI("EXEC GetAllSongs");
            return Json(a, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllArtists()
        {
            ArrayList a = db.getAPI("EXEC GetAllArtists");
            return Json(a, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllUsers(string role)
        {
            if (role == "Admin")
            {
                ArrayList a = db.getAPI("EXEC GetAllUsers");
                return Json(a, JsonRequestBehavior.AllowGet);
            }
            return Json(JsonRequestBehavior.DenyGet);
        }

        public JsonResult GetAllGenres()
        {
            ArrayList a = db.getAPI("EXEC GetAllGenres");
            return Json(a, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllAlbums()
        {
            ArrayList a = db.getAPI("EXEC GetAlbumWithArtists");
            return Json(a, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSongById(string id)
        {
            ArrayList a = db.getAPI("EXEC GetSongById " + id);
            return Json(a, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoginUser(string taikhoan, string matkhau)
        {
            ArrayList a = db.getAPI($"exec UserLogin @Username = '{taikhoan}', @Password =  '{matkhau}'");
            return Json(a, JsonRequestBehavior.AllowGet);
        }

        // Songs - Change to  for adding and updating
        
        public JsonResult AddSong(string songName, DateTime releaseDate, string fileName, string thumbnailImage, int genreId, int? albumId)
        {
            try
            {
                db.getAPI($"EXEC AddSong @SongName = N'{songName}', " +
                          $"@ReleaseDate = '{releaseDate:yyyy-MM-dd}', " +
                          $"@FileName = N'{fileName}', " +
                          $"@ThumbnailImage = N'{thumbnailImage}', " +
                          $"@GenreId = {genreId}, " +
                          $"@AlbumId = {(albumId.HasValue ? albumId.Value.ToString() : "NULL")}");
                return Json(new { success = true, message = "Bài hát đã được thêm thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        
        public JsonResult UpdateSong(int songId, string songName, DateTime releaseDate, string fileName, string thumbnailImage, int genreId, int? albumId)
        {
            try
            {
                db.getAPI($"EXEC UpdateSong @SongId = {songId}, " +
                          $"@SongName = N'{songName}', " +
                          $"@ReleaseDate = '{releaseDate:yyyy-MM-dd}', " +
                          $"@FileName = N'{fileName}', " +
                          $"@ThumbnailImage = N'{thumbnailImage}', " +
                          $"@GenreId = {genreId}, " +
                          $"@AlbumId = {(albumId.HasValue ? albumId.Value.ToString() : "NULL")}");
                return Json(new { success = true, message = "Bài hát đã được cập nhật thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        
        public JsonResult DeleteSong(int songId)
        {
            try
            {
                db.getAPI($"EXEC DeleteSong @SongId = {songId}");
                return Json(new { success = true, message = "Bài hát đã được xóa thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        
        public JsonResult AddArtist(string artistName, string bio, string avatarImage)
        {
            try
            {
                db.getAPI($"EXEC AddArtist @ArtistName = N'{artistName}', @Bio = N'{bio}', @AvatarImage = N'{avatarImage}'");
                return Json(new { success = true, message = "Nghệ sĩ đã được thêm thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        
        public JsonResult UpdateArtist(int artistId, string artistName, string bio, string avatarImage)
        {
            try
            {
                db.getAPI($"EXEC UpdateArtist @ArtistId = {artistId}, " +
                          $"@ArtistName = N'{artistName}', " +
                          $"@Bio = N'{bio}', " +
                          $"@AvatarImage = N'{avatarImage}'");
                return Json(new { success = true, message = "Nghệ sĩ đã được cập nhật thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        
        public JsonResult DeleteArtist(int artistId)
        {
            try
            {
                db.getAPI($"EXEC DeleteArtist @ArtistId = {artistId}");
                return Json(new { success = true, message = "Nghệ sĩ đã được xóa thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        
        public JsonResult AddAlbum(string albumName, int artistId, DateTime releaseDate, string coverImage)
        {
            try
            {
                db.getAPI($"EXEC AddAlbum @AlbumName = N'{albumName}', " +
                          $"@ArtistId = {artistId}, " +
                          $"@ReleaseDate = '{releaseDate:yyyy-MM-dd}', " +
                          $"@CoverImage = N'{coverImage}'");
                return Json(new { success = true, message = "Album đã được thêm thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        
        public JsonResult UpdateAlbum(int albumId, string albumName, int artistId, DateTime releaseDate, string coverImage)
        {
            try
            {
                db.getAPI($"EXEC UpdateAlbum @AlbumId = {albumId}, " +
                          $"@AlbumName = N'{albumName}', " +
                          $"@ArtistId = {artistId}, " +
                          $"@ReleaseDate = '{releaseDate:yyyy-MM-dd}', " +
                          $"@CoverImage = N'{coverImage}'");
                return Json(new { success = true, message = "Album đã được cập nhật thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        
        public JsonResult DeleteAlbum(int albumId)
        {
            try
            {
                db.getAPI($"EXEC DeleteAlbum @AlbumId = {albumId}");
                return Json(new { success = true, message = "Album đã được xóa thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        
        public JsonResult AddPlaylist(string playlistName, int userId)
        {
            try
            {
                db.getAPI($"EXEC AddPlaylist @PlaylistName = N'{playlistName}', @UserId = {userId}");
                return Json(new { success = true, message = "Playlist đã được thêm thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        
        public JsonResult UpdatePlaylist(int playlistId, string playlistName)
        {
            try
            {
                db.getAPI($"EXEC UpdatePlaylist @PlaylistId = {playlistId}, @PlaylistName = N'{playlistName}'");
                return Json(new { success = true, message = "Playlist đã được cập nhật thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        
        public JsonResult DeletePlaylist(int playlistId)
        {
            try
            {
                db.getAPI($"EXEC DeletePlaylist @PlaylistId = {playlistId}");
                return Json(new { success = true, message = "Playlist đã được xóa thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult GetUserPlaylists(int songId, int userId)
        {
            DataModel db = new DataModel();

            var playlists = db.get($"SELECT playlist_id, playlist_name FROM Playlists WHERE user_id = {userId}");

            var result = new List<object>();
            foreach (ArrayList playlist in playlists)
            {
                result.Add(new
                {
                    id = playlist[0],
                    name = playlist[1]
                });
            }

            return Json(new { success = true, playlists = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSongsInPlaylist(int playlistId)
        {
            DataModel db = new DataModel();

            var playlists = db.get($"EXEC GetSongsInPlaylist @PlaylistId = {playlistId}");

            return Json(playlists, JsonRequestBehavior.AllowGet);
        }

        
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

        //Users
        
        public JsonResult AddUser(string username, string password, string email, string phoneNumber, string fullname, string role, string adminCode)
        {
            try
            {
                if (adminCode != "ASKALSK")
                {
                    return Json(new { success = false, message = "Bạn không có quyền thêm người dùng." });
                }

                db.getAPI($"EXEC AddNewUser @Username = N'{username}', " +
                          $"@Password = '{password}'), " +
                          $"@Fullname = N'{fullname}', " +
                          $"@Role = N'{role}', " +
                          $"@Email = N'{email}', " +
                          $"@PhoneNumber = N'{phoneNumber}'");
                return Json(new { success = true, message = "Người dùng đã được thêm thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        
        public JsonResult UpdateUser(int userId, string email, string phoneNumber, string fullname, string role, string adminCode)
        {
            try
            {
                if (adminCode != "ASKALSK")
                {
                    return Json(new { success = false, message = "Bạn không có quyền sửa người dùng." });
                }

                db.getAPI($"EXEC UpdateUser @UserId = {userId}, " +
                          $"@Email = N'{email}', " +
                          $"@PhoneNumber = N'{phoneNumber}', " +
                          $"@Fullname = N'{fullname}', " +
                          $"@Role = N'{role}'");
                return Json(new { success = true, message = "Người dùng đã được cập nhật thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult DeleteUser(int userId, string adminCode)
        {
            try
            {
                if (adminCode != "ASKALSK")
                {
                    return Json(new { success = false, message = "Bạn không có quyền xóa người dùng." });
                }

                db.getAPI($"EXEC DeleteUser @UserId = {userId}");
                return Json(new { success = true, message = "Người dùng đã được xóa thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        
        public JsonResult ChangePassword(int userId, string currentPassword, string newPassword)
        {
            try
            {
                var currentPasswordq = db.getScalar($"SELECT password FROM Users WHERE user_id = {userId}").ToString();


                if (currentPasswordq != currentPassword)
                {
                    return Json(new { success = false, message = "Mật khẩu hiện tại không đúng." });
                }

                // Cập nhật mật khẩu mới
                db.getAPI($"EXEC ChangeUserPassword @UserId = {userId}, @NewPassword = '{newPassword}'");

                return Json(new { success = true, message = "Mật khẩu đã được đổi thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
} 