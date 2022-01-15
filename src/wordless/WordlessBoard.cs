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

        if (word.Any(char.IsLower))
        {
            word = word.ToUpper();
        }

        if (!_possibleWords.Contains(word))
        {
            throw new InvalidOperationException("Your word isn't even in your own dictionary");
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

        var choppedGuess = ChopWord(currentAttempt.Guess);

        foreach (var guessCharacter in choppedGuess)
        {
            if (_word.TryGetValue(guessCharacter.Key, out var wordPositions))
            {
                var exacts = guessCharacter.Value.Where(x => wordPositions.Contains(x));
                var partials = guessCharacter.Value.Where(x => !wordPositions.Contains(x));

                var remaining = wordPositions.Count;

                foreach (var entry in exacts)
                {
                    currentAttempt.SetCharacterResult(entry, CharacterState.Correct);
                    --remaining;
                }

                foreach (var entry in partials)
                {
                    var state = remaining-- > 0 ? CharacterState.PresentWrongSpot : CharacterState.NotPresent;
                    currentAttempt.SetCharacterResult(entry, state);
                }
            }
        }

        /*
        var letterCount = new Dictionary<char, int>();
        var resultCache = new List<WordlessResult>();

        for (int guessIndex = 0; guessIndex < currentAttempt.Guess.Length; guessIndex++)
        {
            var result = CharacterState.NotPresent;
            var currentGuessChar = currentAttempt.Guess[guessIndex];
            if (_word.TryGetValue(currentGuessChar, out var positions))
            {
                result = positions.Contains(guessIndex) ? CharacterState.Correct : CharacterState.PresentWrongSpot;

                if (!letterCount.ContainsKey(currentGuessChar))
                {
                    letterCount[currentGuessChar] = 0;
                }

                letterCount[currentGuessChar]++;
            }
            
            var characterResult = new WordlessResult(currentGuessChar, result);
            resultCache.Add(characterResult);
        }
        
        //currentAttempt.AddCharacterResult(characterResult);
        */

        _attempts.Add(currentAttempt);
        return currentAttempt;
    }
}