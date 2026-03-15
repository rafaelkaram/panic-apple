using Godot;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 250.0f;

	private Vector2 _screenSize;
	private AnimatedSprite2D _animatedSprite;

	public override void _Ready()
	{
		_screenSize = GetViewportRect().Size;
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 inputDirection = Vector2.Zero;

		if (Input.IsActionPressed("ui_right")) inputDirection.X += 1;
		if (Input.IsActionPressed("ui_left")) inputDirection.X -= 1;

		if (inputDirection.X != 0)
		{
			_animatedSprite.Play("walk");
			_animatedSprite.FlipH = inputDirection.X < 0;
		}
		else
		{
			_animatedSprite.Play("idle");
		}

		float score = GameManager.Instance?.Score ?? 0;
		float speedMultiplier = 1.0f + (score * 0.001f);

		Velocity = inputDirection * (Speed * speedMultiplier);
		MoveAndSlide();

		Vector2 currentPosition = Position;
		currentPosition.X = Mathf.Clamp(currentPosition.X, 0, _screenSize.X);
		Position = currentPosition;
	}

	public void PlayFeedback(bool isGood)
	{
		if (_animatedSprite == null) return;

		Tween tween = GetTree().CreateTween();
		Color targetColor = isGood ? new Color(0, 1, 0) : new Color(1, 0, 0);

		tween.TweenProperty(_animatedSprite, "modulate", targetColor, 0.1f);
		tween.TweenProperty(_animatedSprite, "modulate", new Color(1, 1, 1), 0.1f);
	}
}
