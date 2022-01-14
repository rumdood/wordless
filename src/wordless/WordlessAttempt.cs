namespace Wordless;

public class WordlessAttempt
{
    private readonly List<WordlessResult> _results;
    public string Guess { get; }

    public IEnumerable<WordlessResult> Result => _results;
    public AttemptStatus Status { get; set; }

    public WordlessAttempt(string guess)
    {
        Status = AttemptStatus.Error;
        Guess = guess.ToUpper();
        _results = new List<WordlessResult>();
    }

    public void AddCharacterResult(WordlessResult result)
    {
        _results.Add(result);
    }
}

public enum AttemptStatus
{
    ValidAttempt,
    GuessNotAllowed,
    Error
}