using System;
using Godot;

public partial class Player : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	internal void OnInitialize(string name)
	{
		playerInfo = new BeingInfo(name);
		playerInfo.NameChangedEvent += OnPlayerNameChanged;
		Name = playerInfo.Name;
		SetUpPlayer();
	}

	private void OnPlayerNameChanged(string newName)
	{
		Name = newName;
		GetNode<Label>("NameLabel").Text = Name;
	}

	[Export]
	public int Speed { get; set; } = 100; // How fast the player will move (pixels/sec).
	public Vector2 ScreenSize; // Size of the game window.
	public BeingInfo PlayerInfo { get { return playerInfo; } }
	public new string Name
	{
		get
		{
			return name;
		}

		protected set
		{
			name = value;
			NameChangedEvent?.Invoke(value);
		}
	}
	public event Action<string> NameChangedEvent;

	private Vector2 syncPos = new Vector2(0, 0);
	private BeingInfo playerInfo;
	private string name;


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero;

		if (Input.IsActionPressed("move_right"))
		{
			velocity += new Vector2(1, 0);
		}

		if (Input.IsActionPressed("move_left"))
		{
			velocity += new Vector2(-1, 0);
		}

		if (Input.IsActionPressed("move_up"))
		{
			velocity += new Vector2(0, -1);
		}

		if (Input.IsActionPressed("move_down"))
		{
			velocity += new Vector2(0, 1);
		}

		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
			animatedSprite2D.Play();
		}
		else
		{
			animatedSprite2D.Stop();
		}

		Position += velocity * (float)delta;
		syncPos = GlobalPosition;
	}

	/// <summary>
	/// Setup player when entering the scene.
	/// </summary>
	/// <param name="name">The name to assign to the player's nametag.</param>
	public void SetUpPlayer()
	{
		GetNode<Label>("NameLabel").Text = Name;
	}
}
