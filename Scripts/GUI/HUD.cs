using Godot;

public partial class HUD : CanvasLayer
{
	public Label NameLabel { get; protected set; }
	private string playerName { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		NameLabel = GetNode<Label>("NameLabel");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		NameLabel.Text = "Name: " + playerName;
	}

	internal void OnInitialize(string name)
	{
		playerName = name;
	}

}
