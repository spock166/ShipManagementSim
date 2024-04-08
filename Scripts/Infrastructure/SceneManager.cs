using Godot;
using System;

public partial class SceneManager : Node2D
{
	[Export]
	private PackedScene playerScene;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		int index = 0;
		foreach(PlayerInfo player in GameManager.Players){
			Player currentPlayer = playerScene.Instantiate<Player>();
			currentPlayer.Name = player.Id.ToString(); // Use Id since unique
			AddChild(currentPlayer);
			foreach( Node2D spawnPoint in GetTree().GetNodesInGroup("SpawnPoints")){
				if(int.Parse(spawnPoint.Name) == index){
					currentPlayer.GlobalPosition = spawnPoint.GlobalPosition;
					GD.Print("Spawning" + currentPlayer.Name);
				}
			}
			index++;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
