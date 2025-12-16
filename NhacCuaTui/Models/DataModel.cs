using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Data;

namespace NhacCuaTui.Models
{
    public class DataModel
    {
        private string connectionString = "Data Source=H310M\\NMTU; Initial Catalog=musicweb1; Integrated Security=True; TrustServerCertificate=True";
        
        public ArrayList get(String sql)
        {
            ArrayList datalist = new ArrayList();

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(sql, connection);
            connection.Open();
            using (SqlDataReader r = command.ExecuteReader())
            {
                while (r.Read())
                {
                    ArrayList row = new ArrayList();
                    for (int i = 0; i < r.FieldCount; i++)
                    {
                        row.Add(r.GetValue(i).ToString());
                    }
                    datalist.Add(row);
                }
            }
            connection.Close();
            return datalist;
        }

        public object getScalar(string sql)
        {
            object result = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                try
                {
                    connection.Open();
                    result = command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error executing scalar query: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }

            return result;
        }


        public ArrayList getAPI(String sql)
        {
            ArrayList datalist = new ArrayList();
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(sql, connection);
            connection.Open();
            using (SqlDataReader r = command.ExecuteReader())
            {
                while (r.Read())
                {
                    ArrayList row = new ArrayList();
                    for (int i = 0; i < r.FieldCount; i++)
                    {
                        row.Add(xulydulieu(r.GetValue(i).ToString()));
                    }
                    datalist.Add(row);
                }
            }
            connection.Close();
            return datalist;
        }
        public string xulydulieu(string text)
        {
            String s = text.Replace(",", "&44;");
            s = s.Replace("\"", "&34;");
            s = s.Replace("'", "&39;");
            s = s.Replace("\r", "");
            s = s.Replace("\n", "\n");
            return s;
        }

        public Dictionary<string, ArrayList> getAllAPI(string sql)
        {
            Dictionary<string, ArrayList> allData = new Dictionary<string, ArrayList>();
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(sql, connection);

            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                int tableIndex = 0;
                string tableName = "";
                do
                {
                    ArrayList tableData = new ArrayList();
                    switch (tableIndex)
                    {
                        case 0:
                            tableName = "Users";
                            break;
                        case 1:
                            tableName = "Genres";
                            break;
                        case 2:
                            tableName = "Artists";
                            break;
                        case 3:
                            tableName = "Albums";
                            break;
                        case 4:
                            tableName = "Songs";
                            break;
                        case 5:
                            tableName = "SongArtists";
                            break;
                        case 6:
                            tableName = "Playlists";
                            break;
                        case 7:
                            tableName = "PlaylistSongs";
                            break;
                        case 8:
                            tableName = "Comments";
                            break;
                    }
                    while (reader.Read())
                    {
                        ArrayList row = new ArrayList();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row.Add(xulydulieu(reader.GetValue(i).ToString()));
                        }
                        tableData.Add(row);
                    }
                    allData.Add($"{tableName}", tableData);
                    tableIndex++;

                } while (reader.NextResult());
            }

            connection.Close();
            return allData;
        }

        public bool ExecuteNonQuery(string sql)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(sql, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing non-query: " + ex.Message);
                return false;
            }
        }

        // Album-related methods
        public ArrayList GetAllAlbums()
        {
            try
            {
                // Use stored procedure GetAlbumWithArtists and add song count
                string sql = @"SELECT 
                    a.album_id, 
                    a.album_name, 
                    ar.artist_name, 
                    a.release_date, 
                    a.cover_image,
                    COUNT(s.song_id) as song_count
                FROM Albums a
                JOIN Artists ar ON a.artist_id = ar.artist_id
                LEFT JOIN Songs s ON a.album_id = s.album_id
                GROUP BY a.album_id, a.album_name, ar.artist_name, a.release_date, a.cover_image
                ORDER BY a.release_date DESC";
                ArrayList result = get(sql);
                return result.Count > 0 ? result : new ArrayList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetAllAlbums: " + ex.Message);
                return new ArrayList();
            }
        }

        public ArrayList GetAlbumById(int albumId)
        {
            try
            {
                string sql = $"EXEC GetAlbumById @AlbumId={albumId}";
                ArrayList result = get(sql);
                return result.Count > 0 ? result : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetAlbumById: " + ex.Message);
                return null;
            }
        }

        public ArrayList GetSongsInAlbum(int albumId)
        {
            try
            {
                string sql = $"EXEC GetSongsInAlbum @AlbumId={albumId}";
                ArrayList result = get(sql);
                return result.Count > 0 ? result : new ArrayList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetSongsInAlbum: " + ex.Message);
                return new ArrayList();
            }
        }

        public ArrayList GetTopAlbums(int top = 10)
        {
            try
            {
                string sql = $"SELECT TOP {top} * FROM Albums ORDER BY CreatedDate DESC";
                return get(sql);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetTopAlbums: " + ex.Message);
                return new ArrayList();
            }
        }

        public ArrayList SearchAlbums(string searchTerm)
        {
            try
            {
                string sql = $"SELECT * FROM Albums WHERE AlbumName LIKE '%{searchTerm}%' OR ArtistName LIKE '%{searchTerm}%'";
                return get(sql);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in SearchAlbums: " + ex.Message);
                return new ArrayList();
            }
        }

        // Playlist-related method for adding album songs
        public bool AddSongToPlaylist(int songId, int playlistId)
        {
            try
            {
                string sql = $"INSERT INTO PlaylistSongs (PlaylistId, SongId) VALUES ({playlistId}, {songId})";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(sql, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in AddSongToPlaylist: " + ex.Message);
                return false;
            }
        }
    }
}