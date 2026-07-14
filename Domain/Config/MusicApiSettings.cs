namespace Domain.Config;

public class MusicApiSettings
{

    public bool Enabled { get; set; }

    public string BaseAddress { get; set; } = "https://itunes.apple.com";
    public int TimeoutSeconds { get; set; } = 30;

    public string[] SearchTerms { get; set; } =
        { "rock", "pop", "jazz", "classical", "electronic", "metal", "hip hop", "folk" };

    public int LimitPerTerm { get; set; } = 25;
}
