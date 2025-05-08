namespace Domain;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int Quantity { get; set; } = 0;
    public float AverageRating { get; set; } = 0.0f;

    public ICollection<Lending> Lendings { get; set; } = new List<Lending>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
}