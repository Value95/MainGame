using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using FishNet.Connection;
using FishNet.Transporting;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Multiplay;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

public class NetworkManager : BaseManager
{
    private static NetworkManager _instance;

    public static NetworkManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 씬에 있는 GameManager 찾기
                _instance = FindObjectOfType<NetworkManager>();

                // 없으면 새로 생성
                if (_instance == null)
                {
                    GameObject go = new GameObject("NetworkManager");
                    _instance = go.AddComponent<NetworkManager>();
                }
            }

            return _instance;
        }
    }

    private string[] _args;
    private bool _isServer = false;
    private string _externalServerIP = "0.0.1.1";
    private ushort _serverPort = 7770;
    
    public override void Prepare()
    {
        _args = System.Environment.GetCommandLineArgs();

        for (int i = -0; i < _args.Length; ++i)
        {
            switch (_args[i])
            {
                case "-Server":
                    _isServer = true;
                    break;
                case "-ip":
                    if (_args.Length > i + 1)
                    {
                        _externalServerIP = _args[i + 1];
                    }
                    break;
                case "-port":
                    if (_args.Length > i + 1)
                    {
                        _serverPort = ushort.Parse(_args[i + 1]);
                    }
                    break;
            }
        }
    }
   
    public override void Run()
    {
        if (_isServer)
        {
            Server.StartServer(_externalServerIP, _serverPort);
        }
        else
        {
            Client.StartClient();
        }
    }

    private void Update()
    {
        if (_isServer)
        {
            Server.UpdateServer();
        }
    }
}

public class Server
{
    private const string NA = "n/a";
    private static string _serverIP;
    private static ushort _serverPort;

    private static IServerQueryHandler _serverQueryHandler;
    private static bool _allowEnter = true;
    
    public static async void StartServer(string externalServerIP, ushort serverPort)
    {
        DebugEx.Log(nameof(StartServer) + $"{externalServerIP}:{serverPort}");
        _serverIP = externalServerIP;
        _serverPort = serverPort;

        DebugEx.Log($"Init Server");
        Camera.main.enabled = false;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        DebugEx.Log($"Start Server Services");
        var option = new InitializationOptions();
        option.SetEnvironmentName("production");
        await UnityServices.InitializeAsync(option);

        DebugEx.Log("Get MatchMaker Payload");
        var serverCallbacks = new MultiplayEventCallbacks();
        serverCallbacks.Allocate -= OnMultiplayerAllocation;
        serverCallbacks.Allocate += OnMultiplayerAllocation;
        serverCallbacks.Deallocate -= OnMultiplayerDeallocation;
        serverCallbacks.Deallocate += OnMultiplayerDeallocation;
        serverCallbacks.Error -= OnMultiplayerError;
        serverCallbacks.Error += OnMultiplayerError;
        serverCallbacks.SubscriptionStateChanged -= OnMultiplayerSubscriptionStateChanged;
        serverCallbacks.SubscriptionStateChanged += OnMultiplayerSubscriptionStateChanged;

        DebugEx.Log($"Subscribe To Server Events");
        var multiplayService = MultiplayService.Instance;
        await multiplayService.SubscribeToServerEventsAsync(serverCallbacks);

        DebugEx.Log("Start Server Query Handler");
        _serverQueryHandler = await multiplayService.StartServerQueryHandlerAsync(100, "TestServer" + Tool.GetRandomString(3), NA, NA, NA);

        DebugEx.Log("Ready Server For Players");
        await multiplayService.ReadyServerForPlayersAsync();
    }

    public static void UpdateServer()
    {
        if (_serverQueryHandler == null)
            return;

        _serverQueryHandler.CurrentPlayers =
            (ushort)FishNet.Managing.NetworkManager.Instances.ElementAt(0).ServerManager.Clients.Count;
        _serverQueryHandler.UpdateServerCheck();
    }
    
    private static async void OnMultiplayerAllocation(MultiplayAllocation allocation)
    {
        DebugEx.Log(nameof(OnMultiplayerAllocation));

        DebugEx.Log("Start Server Connection");
        DebugEx.Log($"Server Port: {_serverPort}");

        var networkManager = FishNet.Managing.NetworkManager.Instances.ElementAt(0);
        networkManager.ServerManager.StartConnection(_serverPort);
        networkManager.ServerManager.OnRemoteConnectionState -= OnRemoteConnectionState;
        networkManager.ServerManager.OnRemoteConnectionState += OnRemoteConnectionState;

        var multiplayService = MultiplayService.Instance;
        var serverConfigs = multiplayService.ServerConfig;
        DebugEx.Log($"Server ID[{serverConfigs.ServerId}]");
        DebugEx.Log($"Server Allocation ID[{serverConfigs.AllocationId}]");
        DebugEx.Log($"Server Port[{serverConfigs.Port}]");
        DebugEx.Log($"Server Query Port[{serverConfigs.QueryPort}]");
        DebugEx.Log($"Server Log Directory[{serverConfigs.ServerLogDirectory}]");

        DebugEx.Log("Get Payload Allocation");
        var matchmakingResult = await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<MatchmakingResults>();

        TryLoopingApproveBackfilling(matchmakingResult.BackfillTicketId).Forget();
    }

    private static void OnMultiplayerDeallocation(MultiplayDeallocation deallocation)
    {
        DebugEx.Log(nameof(OnMultiplayerDeallocation));
    }

    private static void OnMultiplayerError(MultiplayError error)
    {
        DebugEx.Log(nameof(OnMultiplayerError));
    }

    private static void OnMultiplayerSubscriptionStateChanged(MultiplayServerSubscriptionState state)
    {
        DebugEx.Log(nameof(OnMultiplayerSubscriptionStateChanged));
    }

    private static async UniTask TryLoopingApproveBackfilling(string backfillTicketID)
    {
        DebugEx.Log(nameof(TryLoopingApproveBackfilling) + $"backfillTicketID: {backfillTicketID}");

        while (_allowEnter)
        {
            await UniTask.Delay(1000);

            var localBackfillTicket = await MatchmakerService.Instance.ApproveBackfillTicketAsync(backfillTicketID);
            backfillTicketID = localBackfillTicket.Id;
        }
    }

    private static void OnRemoteConnectionState(NetworkConnection connection, RemoteConnectionStateArgs args)
    {
        Debug.Log(nameof(OnRemoteConnectionState) + $" Stage: {args.ConnectionState}, Connection Id: {connection.ClientId}");

        var networkManager = FishNet.Managing.NetworkManager.Instances.ElementAt(0);

        switch (args.ConnectionState)
        {
            case RemoteConnectionState.Stopped:
                if (networkManager.ServerManager.Clients.Count == 0)
                {
                    Debug.LogError("Server Stopp by all clients disconnecting");
                    Application.Quit();
                    break;
                }
                break;
        }
    }

}

public class Client
{
    private static string _ticketId;
    private static MultiplayAssignment _multiplayAssignment;

    private static string PlayerID
    {
        get { return AuthenticationService.Instance.PlayerId; }
    }

    public static async Task StartClient()
    {
        DebugEx.Log(nameof(StartClient));

        await InitializeAndSignIn();

        await CreateTicket();
    }

    private static async UniTask InitializeAndSignIn()
    {
        Debug.Log(nameof(InitializeAndSignIn));

        var serverProfileName = $"User{Tool.GetRandomString(10)}";

        var initOptions = new InitializationOptions();
        initOptions.SetProfile(serverProfileName);
        await UnityServices.InitializeAsync(initOptions);

        await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync($"{serverProfileName}", "Password1!");

        Debug.Log($"Signed in as {serverProfileName}");
    }

    private static async UniTask CreateTicket()
    {
        DebugEx.Log(nameof(CreateTicket));

        var options = new CreateTicketOptions("NetworkTestQueue01");
        // var options = new CreateTicketOptions();

        var players = new List<Player>
        {
            new Player(
                PlayerID,
                new MatchData
                {
                    EnterCode = "Enter"
                }
            )
        };

        var ticketResponse = await MatchmakerService.Instance.CreateTicketAsync(players, options);
        _ticketId = ticketResponse.Id;

        await PollTicketStatus();
    }

    private static async UniTask PollTicketStatus()
    {
        DebugEx.Log(nameof(PollTicketStatus));
        
        _multiplayAssignment = null;
        var gotAssignment = false;

        do
        {
            await UniTask.Delay(1000);
            var ticketStatus = await MatchmakerService.Instance.GetTicketAsync(_ticketId);
            if (ticketStatus == null)
            {
                continue;
            }

            if (ticketStatus.Type == typeof(MultiplayAssignment))
            {
                _multiplayAssignment = ticketStatus.Value as MultiplayAssignment;
            }

            DebugEx.Log($"Ticket Status : {_multiplayAssignment.Status}, Message : {_multiplayAssignment.Message}");

            switch (_multiplayAssignment.Status)
            {
                case MultiplayAssignment.StatusOptions.Found:
                    gotAssignment = true;
                    StartClientConnection(_multiplayAssignment);
                    break;
                case MultiplayAssignment.StatusOptions.InProgress:
                    break;
                case MultiplayAssignment.StatusOptions.Failed:
                    gotAssignment = true;
                    DebugEx.Log("Failed to get assignment");
                    break;
                case MultiplayAssignment.StatusOptions.Timeout:
                    gotAssignment = true;
                    DebugEx.Log("Timed Out");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        } while (gotAssignment == false);
    }

    private static void StartClientConnection(MultiplayAssignment assignment)
    {
        DebugEx.Log(nameof(StartClientConnection));
        
        DebugEx.Log($"Ticket assigned: {assignment.Ip}:{assignment.Port}");

        var networkManager = FishNet.Managing.NetworkManager.Instances.ElementAt(0);

        networkManager.ClientManager.StartConnection(assignment.Ip, (ushort)assignment.Port);
    }

}

[System.Serializable]
public class MatchData
{
    public string EnterCode { get; set; }
}

public class Tool
{
    public static string GetRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[UnityEngine.Random.Range(0, s.Length)]).ToArray());
    }
}