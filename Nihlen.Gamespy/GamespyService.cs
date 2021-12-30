using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Nihlen.Gamespy;

public interface IGamespyService
{
    Task<ServerInfo> QueryServerAsync(IPAddress ipAddress, int queryPort);
}

public class Gamespy3Service : IGamespyService
{
    // Reference: https://bf2tech.uturista.pt/index.php/GameSpy_Protocol
    public async Task<ServerInfo> QueryServerAsync(IPAddress ipAddress, int queryPort)
    {
        var endpoint = new IPEndPoint(ipAddress, queryPort);
        using var client = new UdpClient();

        client.Connect(endpoint);
        if (!client.Client.Connected)
            throw new Exception($"Could not connect to {ipAddress}:{queryPort} (UDP)");

        var stopWatch = Stopwatch.StartNew();
        var bytes = new byte[] { 0xFE, 0xFD, 0x00, 0x10, 0x20, 0x30, 0x40, 0xFF, 0xFF, 0xFF, 0x01 };
        var sendCancellationToken = new CancellationTokenSource(2_500);
        await client.SendAsync(bytes, bytes.Length).WithCancellation(sendCancellationToken.Token);

        var messages = new List<byte[]>();

        var isMessageComplete = false;
        var receiveCancellationToken = new CancellationTokenSource(2_500);
        while (!isMessageComplete && !receiveCancellationToken.IsCancellationRequested)
        {
            var result = await client.ReceiveAsync().WithCancellation(receiveCancellationToken.Token);
            messages.Add(result.Buffer);
            isMessageComplete = (result.Buffer[14] & (1 << 7)) != 0;
        }

        var elapsedTime = stopWatch.ElapsedMilliseconds;

        if (!isMessageComplete)
            throw new OperationCanceledException($"Server receive was cancelled, but did not receive last message: {client.Available}");

        const int headerLength = 15;
        var payloads = messages.Select(m => m.Skip(headerLength).Take(m.Length - headerLength).ToArray());

        try
        {
            var parsedServer = Parse(payloads);
            var serverInfo = ServerInfo.Create(ipAddress, queryPort, parsedServer.ServerData, parsedServer.PlayerData, parsedServer.TeamData);
            serverInfo.ResponseTime = elapsedTime;
            return serverInfo;
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to parse server response from {ipAddress}: {Encoding.UTF8.GetString(messages.SelectMany(m => m.Skip(headerLength).Take(m.Length - headerLength)).ToArray())}", e);
        }
    }

    public static ParsedServer Parse(IEnumerable<byte[]> payloads)
    {
        var serverData = new Dictionary<string, string>();
        var playerData = new Dictionary<string, IList<string>>();
        var teamData = new Dictionary<string, IList<string>>();

        foreach (var payload in payloads)
        {
            var reader = new GamespyReader(payload);
            while (reader.HasData)
            {
                var type = reader.ReadByte();
                if (type == 0)
                {
                    ParseServerInformation(reader);
                }
                else if (type == 1)
                {
                    ParsePlayerInformation(reader);
                }
                else if (type == 2)
                {
                    ParseTeamInformation(reader);
                }
            }
        }

        void ParseServerInformation(GamespyReader reader)
        {
            while (reader.PeekByte() != 0)
            {
                var key = reader.ReadNextParam();
                var value = reader.ReadNextParam();
                serverData.Add(key, value);
            }

            reader.ReadByte();
        }

        void ParsePlayerInformation(GamespyReader reader)
        {
            while (reader.PeekByte() != 0)
            {
                var key = reader.ReadNextParam();
                if (!playerData.ContainsKey(key))
                {
                    playerData.Add(key, new List<string>());
                }

                var offset = reader.ReadByte();

                while (reader.PeekByte() != 0)
                {
                    if (playerData[key].Count <= offset)
                    {
                        playerData[key].Add(string.Empty);
                    }

                    playerData[key][offset++] = reader.ReadNextParam();
                }

                reader.ReadByte();
            }

            reader.ReadByte();
        }

        void ParseTeamInformation(GamespyReader reader)
        {
            while (reader.PeekByte() != 0)
            {
                var key = reader.ReadNextParam();
                if (!teamData.ContainsKey(key))
                {
                    teamData.Add(key, new List<string>());
                }

                var offset = reader.ReadByte();

                while (reader.PeekByte() != 0)
                {
                    if (teamData[key].Count <= offset)
                    {
                        teamData[key].Add(string.Empty);
                    }

                    teamData[key][offset++] = reader.ReadNextParam();
                }

                reader.ReadByte();
            }

            reader.ReadByte();
        }

        return new ParsedServer
        {
            ServerData = serverData,
            PlayerData = playerData,
            TeamData = teamData
        };
    }

    public class ParsedServer
    {
        public Dictionary<string, string> ServerData { get; set; }
        public Dictionary<string, IList<string>> PlayerData { get; set; }
        public Dictionary<string, IList<string>> TeamData { get; set; }
    }
}

//public class ServerModel
//{
//    public bool IsOnline { get; set; }
//    public string Name { get; set; }
//    public string GameName { get; set; }
//    public string MapName { get; set; }
//    public int NumPlayers { get; set; }
//    public int MaxPlayers { get; set; }
//    public string GameMode { get; set; }
//    public bool HasPassword { get; set; }
//    public string HostIp { get; set; }
//    public int HostPort { get; set; }
//    public int QueryPort { get; set; }

//    // bf2_
//    public bool IsDedicated { get; set; }
//    public bool IsRanked { get; set; }
//    public bool HasAntiCheat { get; set; }
//    public string OperatingSystem { get; set; }
//    public bool HasAutoRecord { get; set; }
//    public string DemoIndexUri { get; set; }
//    public bool HasVoip { get; set; }
//    public string SponsorText { get; set; }
//    public string CommunityLogoUri { get; set; }
//    public string Team1 { get; set; }
//    public string Team2 { get; set; }
//    public int NumBots { get; set; }
//    public int MapSize { get; set; }
//    public bool HasGlobalUnlocks { get; set; }
//    public int ReservedSlots { get; set; }
//    public bool HasNoVehicles { get; set; }

//    public Player[] Players { get; set; }

//    public long ResponseTime { get; set; }
//    public string CountryCode { get; set; }

//    public ServerModel(string uri, bool isOnline)
//    {
//        Name = uri;
//        IsOnline = isOnline;
//        Players = new Player[0];

//        try
//        {
//            var parts = uri.Split(':');
//            HostIp = parts[0];
//            var queryPort = 0;
//            int.TryParse(parts[1], out queryPort);
//            QueryPort = queryPort;
//        }
//        catch (Exception)
//        {
//            // Ignore
//        }
//    }

//    public ServerModel(ServerInfo info)
//    {
//        IsOnline = info.IsOnline;
//        Name = info.Name;
//        QueryPort = info.QueryPort;
//        HasGlobalUnlocks = info.HasGlobalUnlocks;
//        ReservedSlots = info.ReservedSlots;
//        MapSize = info.MapSize;
//        HasAutoRecord = info.HasAutoRecord;
//        IsRanked = info.IsRanked;
//        HasAntiCheat = info.HasAntiCheat;
//        HasNoVehicles = info.HasNoVehicles;
//        NumPlayers = info.NumPlayers;
//        MapName = info.MapName;
//        NumBots = info.NumBots;
//        DemoIndexUri = info.DemoIndexUri?.ToString();
//        IsDedicated = info.IsDedicated;
//        HasVoip = info.HasVoip;
//        SponsorText = info.SponsorText;
//        ResponseTime = info.ResponseTime;
//        CommunityLogoUri = info.CommunityLogoUri?.ToString();
//        Team1 = info.Team1;
//        Team2 = info.Team2;
//        OperatingSystem = info.OperatingSystem;
//        GameMode = info.GameMode;
//        GameName = info.GameName;
//        HasPassword = info.HasPassword;
//        HostIp = info.HostIp?.ToString();
//        HostPort = info.HostPort;
//        MaxPlayers = info.MaxPlayers;
//        Players = info.Players;
//    }
//}
