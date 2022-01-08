namespace Wordless;

public class WordlessBoard
{
    private readonly Dictionary<char, HashSet<int>> _word;
    private readonly List<WordlessAttempt> _attempts;
    private readonly int _maxLength;
    public int MaxAttempts { get; private set; }

    public WordlessBoard(string word, int maxAttempts = 6)
    {
        MaxAttempts = maxAttempts;

        _maxLength = word.Length;
        _attempts = new List<WordlessAttempt>(MaxAttempts);

        _word = SetWord(word);
    }

    public WordlessAttempt[] History => _attempts.ToArray(); 

    private static Dictionary<char, HashSet<int>> SetWord(string word)
    {
        var storage = new Dictionary<char, HashSet<int>>(word.Length);

        for (int i = 0; i < word.Length; i++)
        {
            var currentChar = char.ToUpperInvariant(word[i]);
            if (!storage.ContainsKey(currentChar))
            {
                storage[currentChar] = new HashSet<int>();
            }

            storage[currentChar].Add(i);
        }

        return storage;
    }

    public WordlessAttempt MakeGuess(string guess)
    {
        if (_attempts.Count == MaxAttempts)
        {
            throw new InvalidOperationException("Too many attempts");
        }
        
        if (guess.Length != _maxLength)
        {
            throw new InvalidOperationException("Word not correct size");
        }

        var currentAttempt = new WordlessAttempt(guess);

        for (int guessIndex = 0; guessIndex < guess.Length; guessIndex++)
        {
            var result = CharacterState.NotPresent;
            var currentGuessChar = char.ToUpperInvariant(guess[guessIndex]);
            if (_word.TryGetValue(currentGuessChar, out var positions))
            {
                result = positions.Contains(guessIndex) ? CharacterState.Correct : CharacterState.PresentWrongSpot;
            }

            var characterResult = new WordlessResult(currentGuessChar, result);
            currentAttempt.AddCharacterResult(characterResult);
        }

        _attempts.Add(currentAttempt);
        return currentAttempt;
    }
}