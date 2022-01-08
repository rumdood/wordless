// See https://aka.ms/new-console-template for more information

using Wordless;
const ConsoleColor DefaultForeColor = ConsoleColor.White;
const ConsoleColor DefaultBackColor = ConsoleColor.Black;

Console.ForegroundColor = DefaultForeColor;
Console.BackgroundColor = DefaultBackColor;

Console.WriteLine("Welcome to worthless wordless");

int maxAttempts = GetNumericInputFromUser(6, "Maximum Tries");

var word = "";

while (string.IsNullOrEmpty(word))
{
    Console.Write("Set the word:");
    word = Console.ReadLine();
}

var board = new WordlessBoard(word, maxAttempts);

Console.WriteLine("OK, this is totally dumb because you know the word, but let's play anyway");

bool success = false;

while (board.History.Length < maxAttempts)
{
    Console.Write("Enter your guess:");
    var guess = Console.ReadLine();
    var currentAttempt = board.MakeGuess(guess);

    foreach (var attempt in board.History)
    {
        PrintResult(attempt);
        Console.WriteLine();
    }

    if (currentAttempt.Result.All(x => x.State == CharacterState.Correct))
    {
        success = true;

        for (int att = board.History.Length; att < maxAttempts; att++)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            foreach (var c in guess)
            {
                Console.Write(" ");
            }
            Console.WriteLine();
        }

        Console.BackgroundColor = DefaultBackColor;
        break;
    }
}

if (success)
{
    Console.WriteLine($"Congratulations! You solved it in {board.History.Length} tries!");
}
else
{
    Console.WriteLine($"Better luck next time! The word was {word}");
}

static int GetNumericInputFromUser(int defaultValue, string prompt)
{
    int currentValue = 0;
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
            if (!int.TryParse(response, out currentValue))
            {
                currentValue = 0;
                Console.WriteLine("Please enter a valid number greater than zero or accept the default by pressing enter");
            }
        }
    }

    return currentValue;
}

static void PrintResult(WordlessAttempt attempt)
{
    foreach (var (key, value) in attempt.Result)
    {
        var color = value switch
        {
            CharacterState.NotPresent => ConsoleColor.Gray,
            CharacterState.PresentWrongSpot => ConsoleColor.Yellow,
            CharacterState.Correct => ConsoleColor.Green,
            _ => throw new InvalidOperationException("What?")
        };

        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = color;
        Console.Write($"{key}");
        Console.BackgroundColor = DefaultBackColor;
        Console.ForegroundColor = DefaultForeColor;
    }
}
