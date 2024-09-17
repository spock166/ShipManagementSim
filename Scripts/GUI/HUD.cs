using Godot;

public partial class HUD : CanvasLayer
{
	#region Public properties
	/// <summary>
	/// The name label in the HUD.
	/// </summary>
	public Label NameLabel { get; protected set; }
	#endregion Public properties

	#region Private fields
	/// <summary>
	/// The player's name.
	/// </summary>
	private string playerName { get; set; }
	#endregion Private fields

	/// <summary>
	/// Called when the node enters the scene tree for the first time.
	/// </summary>
	public override void _Ready()
	{
		NameLabel = GetNode<Label>("NameLabel");
	}

	/// <summary>
	/// Called every frame.
	/// </summary>
	/// <param name="delta">The time elapsed since the previous frame.</param>
	public override void _Process(double delta)
	{
		NameLabel.Text = "Name: " + playerName;
	}

	/// <summary>
	/// Sets the initial state of the HUD.
	/// </summary>
	/// <param name="name">The player's name.</param>
	internal void OnInitialize(string name)
	{
		playerName = name;
	}

}
