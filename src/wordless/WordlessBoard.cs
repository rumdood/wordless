namespace Wordless;

public class WordlessBoard
{
    private readonly HashSet<string> _possibleWords;
    private readonly HashSet<string> _possibleGuesses;
    private readonly Dictionary<char, HashSet<int>> _word;
    private readonly List<WordlessAttempt> _attempts;
    public int MaxAttempts { get; private set; }
    
    public WordlessBoard(IReadOnlyList<string> possibleWords, 
        IEnumerable<string> possibleGuesses, 
        string word = "",
        int maxAttempts = 6)
    {
        MaxAttempts = maxAttempts;
        _possibleWords = new HashSet<string>(possibleWords);

        if (string.IsNullOrEmpty(word))
        {
            var rnd = new Random();
            word = possibleWords[rnd.Next(0, possibleWords.Count - 1)];
        }
        else
        {
            if (word.Any(char.IsLower))
            {
                word = word.ToUpper();
            }

            if (!_possibleWords.Contains(word))
            {
                throw new InvalidOperationException("Your word isn't even in your own dictionary");
            }
        }

        _possibleGuesses = new HashSet<string>(possibleWords.Union(possibleGuesses));
        _attempts = new List<WordlessAttempt>(MaxAttempts);
        _word = ChopWord(word);
    }

    public WordlessAttempt[] History => _attempts.ToArray();

    private static Dictionary<char, HashSet<int>> ChopWord(string word)
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

    private void ProcessAttemptSimple(WordlessAttempt currentAttempt)
    {
        var counts = new Dictionary<char, int>();

        foreach (var (key, value) in _word)
        {
            counts[key] = value.Count;
        }

        for (int i = 0; i < currentAttempt.Guess.Length; i++)
        {
            if (!_word.TryGetValue(currentAttempt.Guess[i], out var wordPositions))
            {
                continue;
            }

            if (wordPositions.Contains(i))
            {
                currentAttempt.SetCharacterResult(i, CharacterState.Correct);
                counts[currentAttempt.Guess[i]] -= 1;
            }
        }
        
        for (int i = 0; i < currentAttempt.Guess.Length; i++)
        {
            if (!_word.TryGetValue(currentAttempt.Guess[i], out var wordPositions))
            {
                continue;
            }

            if (counts[currentAttempt.Guess[i]] > 0 && !wordPositions.Contains(i))
            {
                currentAttempt.SetCharacterResult(i, CharacterState.PresentWrongSpot);
                counts[currentAttempt.Guess[i]] -= 1;
            }
        }
    }

    private void ProcessAttemptChopped(WordlessAttempt currentAttempt)
    {
        var choppedGuess = ChopWord(currentAttempt.Guess);

        foreach (var guessCharacter in choppedGuess)
        {
            if (!_word.TryGetValue(guessCharacter.Key, out var wordPositions))
            {
                continue;
            }
            
            var charCount = wordPositions.Count;

            foreach (var entry in guessCharacter.Value)
            {
                if (!wordPositions.Contains(entry))
                {
                    continue;
                }
                
                currentAttempt.SetCharacterResult(entry, CharacterState.Correct);
                --charCount;
            }

            if (charCount == 0)
            {
                continue;
            }

            foreach (var entry in guessCharacter.Value)
            {
                if (wordPositions.Contains(entry))
                {
                    continue;
                }
                
                currentAttempt.SetCharacterResult(entry, CharacterState.PresentWrongSpot);
                if (--charCount == 0)
                {
                    break;
                }
            }
        }
    }

    public WordlessAttempt MakeGuess(string guess)
    {
        var currentAttempt = new WordlessAttempt(guess);

        if (_attempts.Count == MaxAttempts)
        {
            currentAttempt.Status = AttemptStatus.Error;
            return currentAttempt;
        }

        if (!_possibleGuesses.Contains(currentAttempt.Guess))
        {
            currentAttempt.Status = AttemptStatus.GuessNotAllowed;
            return currentAttempt;
        }

        currentAttempt.Status = AttemptStatus.ValidAttempt;
        ProcessAttemptSimple(currentAttempt);
        //ProcessAttemptChopped(currentAttempt);

        _attempts.Add(currentAttempt);
        return currentAttempt;
    }
}