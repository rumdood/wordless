using System.Text.Json;
using Wordless;
const ConsoleColor defaultForeColor = ConsoleColor.White;
const ConsoleColor defaultBackColor = ConsoleColor.Black;

Console.ForegroundColor = defaultForeColor;
Console.BackgroundColor = defaultBackColor;

Console.WriteLine("Welcome to worthless wordless");

var maxAttempts = GetNumericInputFromUser(6, "Maximum Tries");

var word = "";

Console.Write("Set the word (leave blank for random):");
word = Console.ReadLine() ?? "";

Console.WriteLine("Loading word lists");

var wordLists = GetWordsFromFile("words.json");

var board = new WordlessBoard(possibleWords: wordLists.possibleWords, 
    possibleGuesses: wordLists.possibleGuesses,
    word: word,
    maxAttempts: maxAttempts);

Console.WriteLine("LET'S PLAY");

var success = false;

while (board.History.Length < maxAttempts && !success)
{
    Console.Write("Enter your guess:");
    var guess = Console.ReadLine();

    if (string.IsNullOrEmpty(guess))
    {
        Console.WriteLine("You can't guess blank");
        continue;
    }
    
    var currentAttempt = board.MakeGuess(guess);

    if (currentAttempt.Status == AttemptStatus.GuessNotAllowed)
    {
        Console.WriteLine("{0} is not a valid guess", currentAttempt.Guess);
        continue;
    }

    if (currentAttempt.Status == AttemptStatus.Error)
    {
        Console.WriteLine("There was an error processing your guess. Sorry about that");
        continue;
    }

    foreach (var attempt in board.History)
    {
        PrintResult(attempt);
        Console.WriteLine();
    }

    if (currentAttempt.Result.All(x => x.State == CharacterState.Correct))
    {
        success = true;  
    }
}

Console.BackgroundColor = defaultBackColor;
Console.WriteLine(success
    ? $"Congratulations! You solved it in {board.History.Length} tries!"
    : $"Better luck next time! The word was {board.Word}");

static int GetNumericInputFromUser(int defaultValue, string prompt)
{
    var currentValue = 0;
    while (currentValue == 0)
    {
        Console.Write($"{prompt} (default {defaultValue}):");
        var response = Console.ReadLine();

        if (string.IsNullOrEmpty(response))
        {
            currentValue = defaultValue;
        }
        else
        {
            if (int.TryParse(response, out currentValue))
            {
                continue;
            }
            
            currentValue = 0;
            Console.WriteLine("Please enter a valid number greater than zero or accept the default by pressing enter");
        }
    }

    return currentValue;
}

static void PrintResult(WordlessAttempt attempt)
{
    foreach (var f in attempt.Result)
    {
        var color = f.State switch
        {
            CharacterState.NotPresent => ConsoleColor.Gray,
            CharacterState.PresentWrongSpot => ConsoleColor.DarkYellow,
            CharacterState.Correct => ConsoleColor.DarkGreen,
            _ => throw new InvalidOperationException("What?")
        };

        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = color;
        Console.Write($"{f.Character}");
        Console.BackgroundColor = defaultBackColor;
        Console.ForegroundColor = defaultForeColor;
    }
}

static WordLists GetWordsFromFile(string path)
{
    if (!File.Exists(path))
    {
        throw new FileNotFoundException();
    }
    
    var json = File.ReadAllText(path);
    var lists = JsonSerializer.Deserialize<WordLists>(json);
    return lists ?? throw new InvalidOperationException("No word lists found");
}

internal class WordLists
{
    public string[] possibleWords { get; set; }
    public string[] possibleGuesses { get; set; }
}
