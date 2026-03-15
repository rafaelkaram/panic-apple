using Godot;

public abstract partial class FallingItem : Area2D
{
	[Export] public float FallSpeed = 150.0f;
	[Export] public int ScoreValue { get; set; } = 0;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	public override void _Process(double delta)
	{
		Vector2 currentPosition = Position;
		currentPosition.Y += FallSpeed * (float)delta;
		Position = currentPosition;

		if (Position.Y > 700)
		{
			QueueFree();
		}
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is Player)
		{
			GameManager.Instance.AddScore(ScoreValue);
			OnCaptured();
			QueueFree();
		}
	}

	protected abstract void OnCaptured();
}
