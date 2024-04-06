using Godot;
using System;

public partial class Player : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	ScreenSize = GetViewportRect().Size;
	}

	[Export]
    	public int Speed { get; set; } = 50; // How fast the player will move (pixels/sec).

    	public Vector2 ScreenSize; // Size of the game window.

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero;

		if(Input.IsActionPressed("move_right")){
			velocity += new Vector2(1, 0);
		}

		if(Input.IsActionPressed("move_left")){
			velocity += new Vector2(-1, 0);
		}

		if(Input.IsActionPressed("move_up")){
			velocity += new Vector2(0, -1);
		}

		if(Input.IsActionPressed("move_down")){
			velocity += new Vector2(0, 1);
		}

		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if(velocity.Length() > 0){
			velocity = velocity.Normalized() * Speed;
			animatedSprite2D.Play();
		}else{
			animatedSprite2D.Stop();
		}

		Position += velocity * (float)delta;
		Position = new Vector2(
			x:Mathf.Clamp(Position.X,0,ScreenSize.X),
			y:Mathf.Clamp(Position.Y,0,ScreenSize.Y)
		);
	}
}
