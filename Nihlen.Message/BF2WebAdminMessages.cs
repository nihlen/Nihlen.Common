// ReSharper disable All
namespace Nihlen.Message;

// TODO: keep messages in sync with Nihlen.Common / github nuget?

/// <summary>
/// Start a stream a game or optional match
/// </summary>
public record StartGameStream
{
    public string GameServerIp { get; init; }
    public int GameServerPort { get; init; }
    public string GameServerPassword { get; init; }
    public int GameQueryPort { get; init; }
    public Guid? MatchId { get; init; }
    public string? MatchMode { get; init; }
}

/// <summary>
/// Stop stream for the specified server
/// </summary>
public record StopGameStream
{
    public string GameServerIp { get; init; }
    public int GameServerPort { get; init; }
    public Guid? MatchId { get; init; }
}
