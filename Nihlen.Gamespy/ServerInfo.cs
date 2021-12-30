using System.Net;

namespace Nihlen.Gamespy;

public class ServerInfo
{
    public bool IsOnline { get; set; }
    public string Name { get; set; }
    public string GameName { get; set; }
    public string GameVersion { get; set; }
    public string MapName { get; set; }
    public string GameType { get; set; }
    public string GameVariant { get; set; }
    public int NumPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public string GameMode { get; set; }
    public bool HasPassword { get; set; }
    public int TimeLimit { get; set; }
    public int RoundTime { get; set; }
    public IPAddress HostIp { get; set; }
    public int HostPort { get; set; }
    public int QueryPort { get; set; }
    public bool IsDedicated { get; set; }
    public bool IsRanked { get; set; }
    public bool HasAntiCheat { get; set; }
    public string OperatingSystem { get; set; }
    public bool HasAutoRecord { get; set; }
    public Uri DemoIndexUri { get; set; }
    public Uri DemoDownloadUri { get; set; }
    public bool HasVoip { get; set; }
    public bool IsAutobalanced { get; set; }
    public bool HasFriendlyFire { get; set; }
    public string TeamkillMode { get; set; }
    public int StartDelay { get; set; }
    public double SpawnTime { get; set; }
    public string SponsorText { get; set; }
    public Uri SponsorLogoUri { get; set; }
    public Uri CommunityLogoUri { get; set; }
    public int ScoreLimit { get; set; }
    public int TicketRatio { get; set; }
    public double TeamRatio { get; set; }
    public string Team1 { get; set; }
    public string Team2 { get; set; }
    public int NumBots { get; set; }
    public bool IsPure { get; set; }
    public int MapSize { get; set; }
    public bool HasGlobalUnlocks { get; set; }
    public double Fps { get; set; }
    public int ReservedSlots { get; set; }
    public bool HasNoVehicles { get; set; }

    public Player[] Players { get; set; } = Array.Empty<Player>();

    public long ResponseTime { get; set; }

    public static ServerInfo Create(IPAddress ipAddress, int queryPort, Dictionary<string, string> serverData, Dictionary<string, IList<string>> playerData, Dictionary<string, IList<string>> teamData)
    {
        var result = new ServerInfo
        {
            HostIp = ipAddress,
            QueryPort = queryPort
        };

        ApplyServerData(serverData, result);
        ApplyPlayerData(playerData, result);
        ApplyTeamData(teamData, result);

        return result;
    }

    private static void ApplyServerData(Dictionary<string, string> serverData, ServerInfo info)
    {
        info.IsOnline = true;
        info.Name = serverData.GetValue("hostname");
        info.GameName = serverData.GetValue("gamename");
        info.GameVersion = serverData.GetValue("gamever");
        info.MapName = serverData.GetValue("mapname");
        info.GameType = serverData.GetValue("gametype");
        info.GameVariant = serverData.GetValue("gamevariant");
        info.NumPlayers = ParseInt(serverData.GetValue("numplayers"));
        info.MaxPlayers = ParseInt(serverData.GetValue("maxplayers"));
        info.GameMode = serverData.GetValue("gamemode");
        info.HasPassword = ParseBoolean(serverData.GetValue("password"));
        info.TimeLimit = ParseInt(serverData.GetValue("timelimit"));
        info.RoundTime = ParseInt(serverData.GetValue("roundtime"));
        info.HostPort = ParseInt(serverData.GetValue("hostport"));
        info.IsDedicated = ParseBoolean(serverData.GetValue("bf2_dedicated"));
        info.IsRanked = ParseBoolean(serverData.GetValue("bf2_ranked"));
        info.HasAntiCheat = ParseBoolean(serverData.GetValue("bf2_anticheat"));
        info.OperatingSystem = serverData.GetValue("bf2_os");
        info.HasAutoRecord = ParseBoolean(serverData.GetValue("bf2_autorec"));
        info.DemoIndexUri = ParseUri(serverData.GetValue("bf2_d_idx"));
        info.DemoDownloadUri = ParseUri(serverData.GetValue("bf2_d_dl"));
        info.HasVoip = ParseBoolean(serverData.GetValue("bf2_voip"));
        info.IsAutobalanced = ParseBoolean(serverData.GetValue("bf2_autobalanced"));
        info.HasFriendlyFire = ParseBoolean(serverData.GetValue("bf2_friendlyfire"));
        info.TeamkillMode = serverData.GetValue("bf2_tkmode");
        info.StartDelay = ParseInt(serverData.GetValue("bf2_startdelay"));
        info.SpawnTime = ParseDouble(serverData.GetValue("bf2_spawntime"));
        info.SponsorText = serverData.GetValue("bf2_sponsortext");
        info.SponsorLogoUri = ParseUri(serverData.GetValue("bf2_sponsorlogo_url"));
        info.CommunityLogoUri = ParseUri(serverData.GetValue("bf2_communitylogo_url"));
        info.ScoreLimit = ParseInt(serverData.GetValue("bf2_scorelimit"));
        info.TicketRatio = ParseInt(serverData.GetValue("bf2_ticketratio"));
        info.TeamRatio = ParseDouble(serverData.GetValue("bf2_teamratio"));
        info.Team1 = serverData.GetValue("bf2_team1");
        info.Team2 = serverData.GetValue("bf2_team2");
        info.NumBots = ParseInt(serverData.GetValue("bf2_bots"));
        info.IsPure = ParseBoolean(serverData.GetValue("bf2_pure"));
        info.MapSize = ParseInt(serverData.GetValue("bf2_mapsize"));
        info.HasGlobalUnlocks = ParseBoolean(serverData.GetValue("bf2_globalunlocks"));
        info.Fps = ParseDouble(serverData.GetValue("bf2_fps"));
        info.ReservedSlots = ParseInt(serverData.GetValue("bf2_reservedslots"));
        info.HasNoVehicles = ParseBoolean(serverData.GetValue("bf2_novehicles"));
    }

    private static void ApplyPlayerData(Dictionary<string, IList<string>> playerData, ServerInfo serverInfo)
    {
        serverInfo.Players = new Player[playerData["player_"].Count];
        for (var i = 0; i < serverInfo.Players.Length; i++)
        {
            serverInfo.Players[i] = new Player
            {
                Name = playerData["player_"][i],
                Pid = ParseInt(playerData["pid_"][i]),
                TotalScore = ParseInt(playerData["score_"][i]),
                Team = ParseInt(playerData["team_"][i]),
                Kills = ParseInt(playerData["skill_"][i]),
                Deaths = ParseInt(playerData["deaths_"][i]),
                Ping = ParseInt(playerData["ping_"][i]),
                IsBot = ParseBoolean(playerData["AIBot_"][i])
            };
        }
    }

    private static void ApplyTeamData(Dictionary<string, IList<string>> teamData, ServerInfo serverInfo)
    {
        var teams = new Team[teamData["team_t"].Count];
        for (var i = 0; i < teams.Length; i++)
        {
            teams[i] = new Team
            {
                Name = teamData["team_t"][i],
                Score = ParseInt(teamData["score_t"][i]),
            };
        }
    }

    private static int ParseInt(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return 0;

        int.TryParse(value, out var parsedValue);
        return parsedValue;
    }

    private static double ParseDouble(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return 0;

        // TODO: Temp fix for trailing zeros
        value = value.Contains(".") ? value.Split('.')[0] : value;

        double.TryParse(value, out var parsedValue);
        return parsedValue;
    }

    private static bool ParseBoolean(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return value == "1" || value.ToLowerInvariant() == "true";
    }

    private static Uri? ParseUri(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return Uri.TryCreate(value, UriKind.Absolute, out var result) ? result : null;
    }

    public class Player
    {
        public string Name { get; set; }
        public int Pid { get; set; }
        public int Team { get; set; }
        public int TeamScore => TotalScore - 2 * Kills;
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int TotalScore { get; set; }
        public int Ping { get; set; }
        public bool IsBot { get; set; }
        public int Rank { get; set; }
        public string CountryCode { get; set; }
    }

    public class Team
    {
        public string Name { get; set; }
        public int Score { get; set; }
    }
}
