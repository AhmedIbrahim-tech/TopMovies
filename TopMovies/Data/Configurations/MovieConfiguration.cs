using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TopMovies.Entities;

namespace TopMovies.Data.Configurations;

public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.ToTable("Movies");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd();

        builder.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(m => m.Year)
            .IsRequired();

        builder.Property(m => m.Rate)
            .IsRequired();

        builder.Property(m => m.Storeline)
            .IsRequired()
            .HasMaxLength(2500);

        builder.Property(m => m.Poster)
            .IsRequired();

        builder.Property(m => m.GenreId)
            .IsRequired();

        builder.HasOne(m => m.Genre)
            .WithMany()
            .HasForeignKey(m => m.GenreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(m => m.GenreId);
    }
}

