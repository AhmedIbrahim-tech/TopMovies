using System.ComponentModel.DataAnnotations;

namespace TopMovies.ViewModels;

public class GenreFormViewModel
{
    public byte Id { get; set; }

    [Required(ErrorMessage = "Genre name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Genre name must be between 2 and 100 characters")]
    [Display(Name = "Genre Name")]
    public string Name { get; set; } = string.Empty;
}

