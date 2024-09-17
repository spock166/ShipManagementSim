using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class SceneManager : Node2D
{
	[Export]
	private PackedScene playerScene;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Player currentPlayer = playerScene.Instantiate<Player>();

		AddChild(currentPlayer);
		Node2D spawnPoint = GetTree().GetNodesInGroup("SpawnPoints").First() as Node2D;
		currentPlayer.GlobalPosition = spawnPoint.GlobalPosition;
	}
}
