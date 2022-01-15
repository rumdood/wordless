using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Wordless.tests;

public class WordlessTests
{
    public static string[] PossibleWords = new[] {"CAT", "DOG", "SADDLE", "SULLEN", "OWING"};
    public static string[] PossibleGuesses = new[] {"COW", "PUDDLE", "GOING"};
    
    [Theory]
    [MemberData(nameof(GetWordsAndGuesses))]
    public void CharacterResultsPerAttemptAreCorrect(string word, string guess, IList<WordlessResult> expectedResults)
    {
        var board = new WordlessBoard(PossibleWords, PossibleGuesses, word);

        var attempt = board.MakeGuess(guess);

        for (int i = 0; i < expectedResults.Count; i++)
        {
            Assert.Equal(expectedResults[i].Character, attempt.Result.ElementAt(i).Character);
            Assert.Equal(expectedResults[i].State, attempt.Result.ElementAt(i).State);
        }
    }

    public static IEnumerable<object[]> GetWordsAndGuesses()
    {
        yield return new object[]
        {
            "cat", "dog", new List<WordlessResult>
            {
                new('D', CharacterState.NotPresent),
                new('O', CharacterState.NotPresent),
                new('G', CharacterState.NotPresent)
            }
        };

        yield return new object[]
        {
            "cat", "cat", new List<WordlessResult>
            {
                new('C', CharacterState.Correct),
                new('A', CharacterState.Correct),
                new('T', CharacterState.Correct)
            }
        };

        yield return new object[]
        {
            "saddle", "sullen", new List<WordlessResult>
            {
                new('S', CharacterState.Correct),
                new('U', CharacterState.NotPresent),
                new('L', CharacterState.PresentWrongSpot),
                new('L', CharacterState.NotPresent),
                new('E', CharacterState.PresentWrongSpot),
                new('N', CharacterState.NotPresent)
            }
        };

        yield return new object[]
        {
            "CAT", "cat", new List<WordlessResult>
            {
                new('C', CharacterState.Correct),
                new('A', CharacterState.Correct),
                new('T', CharacterState.Correct)
            }
        };

        yield return new object[]
        {
            "cat", "CAT", new List<WordlessResult>
            {
                new('C', CharacterState.Correct),
                new('A', CharacterState.Correct),
                new('T', CharacterState.Correct)
            }
        };
        
        yield return new object[]
        {
            "owing", "going", new List<WordlessResult>
            {
                new('G', CharacterState.NotPresent),
                new('O', CharacterState.PresentWrongSpot),
                new('I', CharacterState.Correct),
                new('N', CharacterState.Correct),
                new('G', CharacterState.Correct)
            }
        };
    }
}