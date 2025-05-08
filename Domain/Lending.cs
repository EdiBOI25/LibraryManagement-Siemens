namespace Domain;

public class Lending
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string BorrowerName { get; set; } = string.Empty;
    public DateTime BorrowDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public int? Rating { get; set; }

    public Book Book { get; set; } = null!;
}