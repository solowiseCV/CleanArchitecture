namespace CleanArchitecture.Domain.Entities
{
    public class Movie
    {
        public Movie()
        {
        }
        public Movie(Guid MovieId, string Name, decimal Cost)
        {
            this.MovieId = MovieId;
            this.Name = Name;
            this.Cost = Cost;
        }
        public Guid MovieId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Cost { get; set; }

    }
}