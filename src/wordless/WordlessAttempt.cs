namespace Wordless;

public class WordlessAttempt
{
    private readonly List<WordlessResult> _results;
    public string Guess { get; }

    public IEnumerable<WordlessResult> Result => _results;

    public WordlessAttempt(string guess)
    {
        Guess = guess;
        _results = new List<WordlessResult>();
    }

    public void AddCharacterResult(WordlessResult result)
    {
        _results.Add(result);
    }
}