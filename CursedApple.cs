using Godot;

public partial class CursedApple : FallingItem
{
	public CursedApple()
	{
		ScoreValue = -25;
		FallSpeed = 180.0f;
	}

	public override void _Ready()
	{
		base._Ready();
		var anim = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		if (anim != null && anim.HasAnimation("falling"))
		{
			anim.Play("falling");
		}
	}

	protected override void OnCaptured()
	{
		GD.Print($"Maçã Amaldiçoada! Pontos: {ScoreValue}");
	}
}
