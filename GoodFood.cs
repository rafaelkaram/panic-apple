using Godot;

public partial class GoodFood : FallingItem
{
	public GoodFood()
	{
		ScoreValue = 10;
		FallSpeed = 150.0f;
	}

	public override void _Ready()
	{
		base._Ready();
	}

	protected override void OnCaptured()
	{
		GD.Print($"Comida capturada! Pontos: +{ScoreValue}");
	}
}
