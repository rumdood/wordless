using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace Wordless.tests;

public class WordlessTests
{
    [Theory]
    [MemberData(nameof(GetWordsAndGuesses))]
    public void CharacterResultsPerAttemptAreCorrect(string word, string guess, IEnumerable<WordlessResult> expectedResults)
    {
        var board = new WordlessBoard(word);

        var attempt = board.MakeGuess(guess);
        Assert.Equal(expectedResults, attempt.Result);
    }

    public static IEnumerable<object[]> GetWordsAndGuesses()
    {
        yield return new object[]
        {
            "cat", "dog", new List<WordlessResult>
            {
                new('d', CharacterState.NotPresent),
                new('o', CharacterState.NotPresent),
                new('g', CharacterState.NotPresent)
            }
        };

        yield return new object[]
        {
            "cat", "cat", new List<WordlessResult>
            {
                new('c', CharacterState.Correct),
                new('a', CharacterState.Correct),
                new('t', CharacterState.Correct)
            }
        };

        yield return new object[]
        {
            "saddle", "sullen", new List<WordlessResult>
            {
                new('s', CharacterState.Correct),
                new('u', CharacterState.NotPresent),
                new('l', CharacterState.PresentWrongSpot),
                new('l', CharacterState.PresentWrongSpot),
                new('e', CharacterState.PresentWrongSpot),
                new('n', CharacterState.NotPresent)
            }
        };
    }
}