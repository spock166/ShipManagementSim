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
		Rpc("StartGame");
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void StartGame()
	{
		Node2D scene = ResourceLoader.Load<PackedScene>("res://Scenes/Areas/main.tscn").Instantiate<Node2D>();
		GetTree().Root.AddChild(scene);
		Hide();
	}
}
