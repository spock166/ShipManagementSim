using System;
using Godot;

public partial class Player : Area2D
{

	#region Exportable Members
	/// <summary>
	/// The player's speed.
	/// </summary>
	[Export]
	public int Speed { get; set; } = 100; // How fast the player will move (pixels/sec).
	#endregion

	#region Public Properties
	/// <summary>
	/// Size of the game window.
	/// </summary>
	public Vector2 ScreenSize;

	/// <summary>
	/// The player's data structure.
	/// </summary>
	public BeingInfo PlayerInfo { get { return playerInfo; } }

	/// <summary>
	/// The player's name.
	/// </summary>
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

	/// <summary>
	/// The name changed event.
	/// </summary>
	public event Action<string> NameChangedEvent;
	#endregion

	#region Private fields
	/// <summary>
	/// The player's info (private).
	/// </summary>
	private BeingInfo playerInfo;

	/// <summary>
	/// The player's name (private).
	/// </summary>
	private string name;
	#endregion Private fields

	/// <summary>
	/// Called when the node enters the scene tree for the first time.
	/// </summary>
	public override void _Ready()
	{

	}

	/// <summary>
	/// Initializes the player object and data structure.
	/// </summary>
	/// <param name="name">The player's name.</param>
	internal void OnInitialize(string name)
	{
		playerInfo = new BeingInfo(name);
		playerInfo.NameChangedEvent += OnPlayerNameChanged;
		Name = playerInfo.Name;
		SetUpPlayer();
	}

	/// <summary>
	/// Runs when player's name is updated.
	/// </summary>
	/// <param name="newName">Player's new name.</param>
	private void OnPlayerNameChanged(string newName)
	{
		Name = newName;
		GetNode<Label>("NameLabel").Text = Name;
	}

	/// <summary>
	/// Called every frame.
	/// </summary>
	/// <param name="delta">The time elapsed since the previous frame.</param>
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
