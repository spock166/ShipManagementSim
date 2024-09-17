using System;
using System.Linq;
using Godot;

public partial class SceneManager : Node2D
{
	#region Exportable Members
	/// <summary>
	/// The player scene.
	/// </summary>
	[Export]
	private PackedScene playerScene;

	/// <summary>
	/// The HUD scene.
	/// </summary>
	[Export]
	private HUD hud;
	#endregion Exportable Members

	/// <summary>
	/// Called when the node enters the scene tree for the first time. 
	/// </summary>
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

	/// <summary>
	/// Update HUD's name label.
	/// </summary>
	/// <param name="name">The player's name.</param>
	private void SetPlayerNameHUD(string name)
	{
		hud.NameLabel.Text = "Name: " + name;
	}

	/// <summary>
	/// Runs when player's name is updated.
	/// </summary>
	/// <param name="newName">Player's new name.</param>
	private void OnPlayerNameChanged(string newName)
	{
		SetPlayerNameHUD(newName);
	}
}
