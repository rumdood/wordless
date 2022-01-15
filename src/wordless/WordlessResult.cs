namespace Wordless;

public interface IWordlessResult
{
    char Character { get; }
    CharacterState State { get; }
}

public record WordlessResult(char Character, CharacterState State) : IWordlessResult;