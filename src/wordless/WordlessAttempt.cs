namespace Wordless;

public class WordlessAttempt
{
    private readonly List<WordlessResult> _results;
    public string Guess { get; }

    public IEnumerable<IWordlessResult> Result => _results;
    public AttemptStatus Status { get; set; }

    public WordlessAttempt(string guess)
    {
        Status = AttemptStatus.Error;
        Guess = guess.ToUpper();
        _results = new List<WordlessResult>(guess.Length);

        foreach (var c in Guess)
        {
            _results.Add(new WordlessResult
            {
                Character = c,
                State = CharacterState.NotPresent
            });
        }
    }

    public void SetCharacterResult(int index, CharacterState state)
    {
        if (index < 0 || index > _results.Count)
        {
            throw new IndexOutOfRangeException(
                "SetCharacterResult cannot access a character out of the scope of the guess");
        }

        _results[index].State = state;
    }

    private class WordlessResult : IWordlessResult
    {
        public char Character { get; init; }
        public CharacterState State { get; set; }
    }
}

public enum AttemptStatus
{
    ValidAttempt,
    GuessNotAllowed,
    Error
}