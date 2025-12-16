using System;

namespace NhacCuaTui.Models
{
    public class NewSongModel
    {
        public string SongName { get; set; }
        public int GenreId { get; set; }
        public string Lyrics { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string FileName { get; set; }
        public string ThumbnailImage { get; set; }
    }
}
