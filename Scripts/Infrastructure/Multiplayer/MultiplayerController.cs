using Godot;
using System;
using System.Data.Common;
using System.Diagnostics;

public partial class MultiplayerController : Control
{
	[Export]
	private int port = 8910;
	[Export]
	private string address = "127.0.0.1";

	private ENetMultiplayerPeer peer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Multiplayer.PeerConnected += PeerConnected;
		Multiplayer.PeerDisconnected += PeerDisconnected;
		Multiplayer.ConnectedToServer += ConnectedToServer;
		Multiplayer.ConnectionFailed += ConnectionFailed;
	}

	/// <summary>
	/// Runs when a player connects and runs on all peers
	/// </summary>
	/// <param name="id">ID of the player that connected.</param>
	private void PeerConnected(long id)
	{
		GD.Print("Player Connected: " + id.ToString());
	}

	/// <summary>
	/// Runs when a player disconnects and runs on all peers
	/// </summary>
	/// <param name="id">ID of the player that disconnected.</param>
	private void PeerDisconnected(long id)
	{
		GD.Print("Player Disconnected: " + id.ToString());
	}

	/// <summary>
	/// Runs when conenction fails and only on client.
	/// </summary>
	private void ConnectionFailed()
	{
		GD.Print("Oh No!  Connection Failed Senpai!");
	}

	/// <summary>
	/// Runs when the conneciton is successful and only runs on the client
	/// </summary>
	private void ConnectedToServer()
	{
		GD.Print("Connected to Server Senpai!  Have Fun!");
		RpcId(1, nameof(SendPlayerInformation), GetNode<LineEdit>("GridContainer/NameEntry").Text, Multiplayer.GetUniqueId());
	}

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

		// Ensure the host gets registered as a player.
		SendPlayerInformation(GetNode<LineEdit>("GridContainer/NameEntry").Text,1);
	}


	public void OnJoinButtonDown()
	{
		peer = new ENetMultiplayerPeer();
		peer.CreateClient(address, port);

		peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = peer;
		GD.Print("Joining Game...");
	}


	public void OnStartButtonDown()
	{
		Rpc(nameof(StartGame));
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void StartGame()
	{
		Node2D scene = ResourceLoader.Load<PackedScene>("res://Scenes/Areas/main.tscn").Instantiate<Node2D>();
		GetTree().Root.AddChild(scene);
		Hide();
	}

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
}
