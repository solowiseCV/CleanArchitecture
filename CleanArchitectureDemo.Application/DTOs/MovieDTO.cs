using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.DTOs;

public class CreateMovieRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    [Range(0, (double)decimal.MaxValue)]
    public decimal Cost { get; set; }
}

public class UpdateMovieRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    [Range(0, (double)decimal.MaxValue)]
    public decimal Cost { get; set; }
}

public class MovieResponse
{
    public Guid MovieId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Cost { get; set; }
}
