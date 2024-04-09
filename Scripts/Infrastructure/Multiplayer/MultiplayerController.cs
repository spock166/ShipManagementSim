using Godot;
using System.Linq;
using System.Net;

public partial class MultiplayerController : Control
{
	[Export]
	private string address = "127.0.0.1";
	[Export]
	private int port = 8910;

	private ENetMultiplayerPeer peer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Multiplayer.PeerConnected += PeerConnected;
		Multiplayer.PeerDisconnected += PeerDisconnected;
		Multiplayer.ConnectedToServer += ConnectedToServer;
		Multiplayer.ConnectionFailed += ConnectionFailed;

		GetNode<LineEdit>("Container/IPEntry").Text = address;
		GetNode<LineEdit>("Container/PortEntry").Text = port.ToString();
	}

	#region  Network Events
	/// <summary>
	/// Runs when a player connects and runs on all peers.
	/// </summary>
	/// <param name="id">ID of the player that connected.</param>
	private void PeerConnected(long id)
	{
		GD.Print("Player Connected: " + id.ToString());
	}

	/// <summary>
	/// Runs when a player disconnects and runs on all peers.
	/// </summary>
	/// <param name="id">ID of the player that disconnected.</param>
	private void PeerDisconnected(long id)
	{
		GD.Print("Player Disconnected: " + id.ToString());
		GameManager.Players.Remove(GameManager.Players.Where(i => i.Id == id).First());
		Godot.Collections.Array<Node> players = GetTree().GetNodesInGroup("Player");
		foreach (Player player in players)
		{
			if (player.Name == id.ToString())
			{
				player.QueueFree();
			}
		}
	}

	/// <summary>
	/// Runs when conenection fails and only on client.
	/// </summary>
	private void ConnectionFailed()
	{
		GD.Print("Oh No!  Connection Failed Senpai!");
	}

	/// <summary>
	/// Runs when the conenection is successful and only runs on the client.
	/// </summary>
	private void ConnectedToServer()
	{
		GD.Print("Connected to Server Senpai!  Have Fun!");
		isConnected = true;
		UpdateMenuOnInput();
		RpcId(1, nameof(SendPlayerInformation), GetNode<LineEdit>("Container/NameEntry").Text, Multiplayer.GetUniqueId());
	}
	#endregion

	#region Button Handling
	/// <summary>
	/// Runs when host button is pressed.  Tries to host a new server with the given information.
	/// </summary>
	public void OnHostButtonDown()
	{
		peer = new ENetMultiplayerPeer();
		Error error = peer.CreateServer(port, maxClients: 8);
		if (error != Error.Ok)
		{
			GD.Print("Error Senpai Cannot Host: " + error.ToString());
			return;
		}
		peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);

		Multiplayer.MultiplayerPeer = peer;
		GD.Print("Waiting For Players...");
		isConnected = true;
		UpdateMenuOnInput();
		// Ensure the host gets registered as a player.
		SendPlayerInformation(GetNode<LineEdit>("Container/NameEntry").Text, 1);
	}

	/// <summary>
	/// Runs when join button is pressed.  Tries to join a server with the given information.
	/// </summary>
	public void OnJoinButtonDown()
	{
		peer = new ENetMultiplayerPeer();
		peer.CreateClient(address, port);

		peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = peer;
		isConnected = true;
		UpdateMenuOnInput();
		GD.Print("Joining Game...");
	}

	/// <summary>
	/// Runs when the start button is pressed.  Starts the game for all connected players.
	/// </summary>
	public void OnStartButtonDown()
	{
		Rpc(nameof(StartGame));
	}

	/// <summary>
	/// RPC call that sends all players to the map.
	/// </summary>
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void StartGame()
	{
		Node2D scene = ResourceLoader.Load<PackedScene>("res://Scenes/Areas/main.tscn").Instantiate<Node2D>();
		GetTree().Root.AddChild(scene);
		Hide();
	}

	/// <summary>
	/// Adds player if they aren't already in the list of players.  If hosting sends the updated player information to all clients.
	/// </summary>
	/// <param name="name">The player name.</param>
	/// <param name="id">The player id.</param>
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void SendPlayerInformation(string name, int id)
	{
		PlayerInfo playerInfo = new PlayerInfo()
		{
			Name = name,
			Id = id,
		};

		if (!GameManager.Players.Contains(playerInfo))
		{
			GameManager.Players.Add(playerInfo);
		}

		if (Multiplayer.IsServer())
		{
			foreach (PlayerInfo player in GameManager.Players)
			{
				Rpc(nameof(SendPlayerInformation), player.Name, player.Id);
			}
		}
	}
	#endregion

	#region Menu Validation
	private bool nameValid = false;
	private bool addrValid = true;
	private bool portValid = true;
	private bool isConnected = false;

	/// <summary>
	/// Runs when the Name field is updated.
	/// </summary>
	/// <param name="inputStr">The updated name.</param>
	public void OnNameEntryChanged(string inputStr)
	{
		nameValid = inputStr != "";
		UpdateMenuOnInput();
	}

	/// <summary>
	/// Runs when the IP Address field is updated.  Verifies if inputStr is a valid IP address.
	/// </summary>
	/// <param name="inputStr">The updated IP address.</param>
	public void OnIPEntryChanged(string inputStr)
	{
		addrValid = IPAddress.TryParse(inputStr, out _);
		UpdateMenuOnInput();
	}

	/// <summary>
	/// Runs when the Port field is updated.  Verifies if inputStr is a valid port (integer).
	/// </summary>
	/// <param name="inputStr">The updated port.</param>
	public void OnPortEntryChanged(string inputStr)
	{
		portValid = int.TryParse(inputStr, out port);
		UpdateMenuOnInput();
	}

	/// <summary>
	/// Determines which items on the menu should be enabled/disabled.
	/// </summary>
	private void UpdateMenuOnInput()
	{
		GetNode<Button>("Container/Host").Disabled = true;
		GetNode<Button>("Container/Join").Disabled = true;
		GetNode<Button>("Container/Start").Disabled = true;

		if (isConnected)
		{
			GetNode<Button>("Container/Start").Disabled = false;
			GetNode<LineEdit>("Container/NameEntry").Editable = false;
			GetNode<LineEdit>("Container/IPEntry").Editable = false;
			GetNode<LineEdit>("Container/PortEntry").Editable = false;
			return;
		}

		if (nameValid && addrValid && portValid)
		{
			GetNode<Button>("Container/Host").Disabled = false;
			GetNode<Button>("Container/Join").Disabled = false;
		}
	}
	#endregion
}
