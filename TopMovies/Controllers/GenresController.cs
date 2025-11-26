using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TopMovies.Data;
using TopMovies.Entities;
using TopMovies.ViewModels;
using NToastNotify;

namespace TopMovies.Controllers;

public class GenresController(ApplicationDbContext context, IToastNotification toastNotification) : Controller
{
    public async Task<IActionResult> Index()
    {
        var genres = await context.Genres.OrderBy(g => g.Name).ToListAsync();
        return View(genres);
    }

    public IActionResult Create()
    {
        var viewModel = new GenreFormViewModel();
        return View("GenreForm", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GenreFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("GenreForm", model);
        }

        var genre = new Genre
        {
            Name = model.Name
        };

        context.Genres.Add(genre);
        await context.SaveChangesAsync();

        toastNotification.AddSuccessToastMessage("Genre created successfully");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(byte? id)
    {
        if (id == null)
            return BadRequest();

        var genre = await context.Genres.FindAsync(id);

        if (genre == null)
            return NotFound();

        var viewModel = new GenreFormViewModel
        {
            Id = genre.Id,
            Name = genre.Name
        };

        return View("GenreForm", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(GenreFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("GenreForm", model);
        }

        var genre = await context.Genres.FindAsync(model.Id);

        if (genre == null)
            return NotFound();

        genre.Name = model.Name;

        await context.SaveChangesAsync();

        toastNotification.AddSuccessToastMessage("Genre updated successfully");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(byte? id)
    {
        if (id == null)
            return BadRequest();

        var genre = await context.Genres.FindAsync(id);

        if (genre == null)
            return NotFound();

        // Check if genre is used by any movies
        var isUsed = await context.Movies.AnyAsync(m => m.GenreId == id);
        
        if (isUsed)
        {
            return BadRequest("Cannot delete genre that is associated with movies.");
        }

        context.Genres.Remove(genre);
        await context.SaveChangesAsync();

        return Ok();
    }
}

