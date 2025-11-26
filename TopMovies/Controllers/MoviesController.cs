namespace TopMovies.Controllers;

public class MoviesController(ApplicationDbContext context, IToastNotification toastNotification) : Controller
{
    private readonly List<string> _allowedExtensions = [".jpg", ".png"];
    private const long MaxAllowedPosterSize = 1048576;

    public async Task<IActionResult> Index()
    {
        var movies = await context.Movies.OrderByDescending(m => m.Rate).ToListAsync();
        return View(movies);
    }

    public async Task<IActionResult> Create()
    {
        var viewModel = new MovieFormViewModel
        {
            Genres = await context.Genres.OrderBy(m => m.Name).ToListAsync()
        };

        return View("MovieForm", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MovieFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Genres = await context.Genres.OrderBy(m => m.Name).ToListAsync();
            return View("MovieForm", model);
        }

        var files = Request.Form.Files;

        if (!files.Any())
        {
            model.Genres = await context.Genres.OrderBy(m => m.Name).ToListAsync();
            ModelState.AddModelError("Poster", "Please select movie poster!");
            return View("MovieForm", model);
        }

        var poster = files.FirstOrDefault();

        if (poster == null)
        {
            model.Genres = await context.Genres.OrderBy(m => m.Name).ToListAsync();
            ModelState.AddModelError("Poster", "Please select movie poster!");
            return View("MovieForm", model);
        }

        if (!_allowedExtensions.Contains(Path.GetExtension(poster.FileName).ToLower()))
        {
            model.Genres = await context.Genres.OrderBy(m => m.Name).ToListAsync();
            ModelState.AddModelError("Poster", "Only .PNG, .JPG images are allowed!");
            return View("MovieForm", model);
        }

        if (poster.Length > MaxAllowedPosterSize)
        {
            model.Genres = await context.Genres.OrderBy(m => m.Name).ToListAsync();
            ModelState.AddModelError("Poster", "Poster cannot be more than 1 MB!");
            return View("MovieForm", model);
        }

        using var dataStream = new MemoryStream();

        await poster.CopyToAsync(dataStream);

        var movies = new Movie
        {
            Title = model.Title,
            GenreId = model.GenreId,
            Year = model.Year,
            Rate = model.Rate,
            Storeline = model.Storeline,
            Poster = dataStream.ToArray()
        };

        context.Movies.Add(movies);
        await context.SaveChangesAsync();

        toastNotification.AddSuccessToastMessage("Movie created successfully");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return BadRequest();

        var movie = await context.Movies.FindAsync(id);

        if (movie == null)
            return NotFound();

        var viewModel = new MovieFormViewModel
        {
            Id = movie.Id,
            Title = movie.Title,
            GenreId = movie.GenreId,
            Rate = movie.Rate,
            Year = movie.Year,
            Storeline = movie.Storeline,
            Poster = movie.Poster,
            Genres = await context.Genres.OrderBy(m => m.Name).ToListAsync()
        };

        return View("MovieForm", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MovieFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Genres = await context.Genres.OrderBy(m => m.Name).ToListAsync();
            return View("MovieForm", model);
        }

        var movie = await context.Movies.FindAsync(model.Id);

        if (movie == null)
            return NotFound();

        var files = Request.Form.Files;

        if (files.Any())
        {
            var poster = files.FirstOrDefault();

            if (poster != null)
            {
                using var dataStream = new MemoryStream();

                await poster.CopyToAsync(dataStream);

                model.Poster = dataStream.ToArray();

                if (!_allowedExtensions.Contains(Path.GetExtension(poster.FileName).ToLower()))
                {
                    model.Genres = await context.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Only .PNG, .JPG images are allowed!");
                    return View("MovieForm", model);
                }

                if (poster.Length > MaxAllowedPosterSize)
                {
                    model.Genres = await context.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Poster cannot be more than 1 MB!");
                    return View("MovieForm", model);
                }

                movie.Poster = model.Poster;
            }
            {
                model.Genres = await context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Only .PNG, .JPG images are allowed!");
                return View("MovieForm", model);
            }

            if (poster.Length > MaxAllowedPosterSize)
            {
                model.Genres = await context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Poster cannot be more than 1 MB!");
                return View("MovieForm", model);
            }

            movie.Poster = model.Poster;
        }

        movie.Title = model.Title;
        movie.GenreId = model.GenreId;
        movie.Year = model.Year;
        movie.Rate = model.Rate;
        movie.Storeline = model.Storeline;

        await context.SaveChangesAsync();

        toastNotification.AddSuccessToastMessage("Movie updated successfully");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return BadRequest();

        var movie = await context.Movies.Include(m => m.Genre).SingleOrDefaultAsync(m => m.Id == id);

        if (movie == null)
            return NotFound();

        return View(movie);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return BadRequest();

        var movie = await context.Movies.FindAsync(id);

        if (movie == null)
            return NotFound();

        context.Movies.Remove(movie);
        await context.SaveChangesAsync();

        return Ok();
    }
}