namespace ImdbScraper;

public class Data
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Year { get; set; }
    public List<string>? Genres { get; set; }
    public string? Rating { get; set; }
    public string? Metascore { get; set; }
    public string? Storyline { get; set; }
    public List<string>? Directors { get; set; }
    public List<string>? Writers { get; set; }
    public List<string>? Stars { get; set; }
    public string? Runtime { get; set; }
    public byte[]? Cover { get; set; }
    public string? Path { get; set; }
}
