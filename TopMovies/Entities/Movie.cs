namespace TopMovies.Entities;

public class Movie
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public int Year { get; set; }

    public double Rate { get; set; }

    public string Storeline { get; set; } = string.Empty;

    public byte[] Poster { get; set; } = Array.Empty<byte>();

    public byte GenreId { get; set; }

    public Genre? Genre { get; set; }
}