using System;
using System.Linq;
using Godot;

public partial class SceneManager : Node2D
{
	[Export]
	private PackedScene playerScene;

	[Export]
	private HUD hud;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Player currentPlayer = playerScene.Instantiate<Player>();
		currentPlayer.OnInitialize("Player");
		currentPlayer.NameChangedEvent += OnPlayerNameChanged;
		AddChild(currentPlayer);
		Node2D spawnPoint = GetTree().GetNodesInGroup("SpawnPoints").First() as Node2D;
		currentPlayer.GlobalPosition = spawnPoint.GlobalPosition;

		hud.OnInitialize(currentPlayer.Name);
	}

	private void SetPlayerNameHUD(string name)
	{
		hud.NameLabel.Text = "Name: " + name;
	}


	private void OnPlayerNameChanged(string newName)
	{
		SetPlayerNameHUD(newName);
	}

	public event Action<string> PlayerNameChangedEvent;
}
