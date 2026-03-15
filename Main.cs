using Godot;
using System.Threading.Tasks;

public partial class Main : Node2D
{
	[Export] public PackedScene GoodFoodScene { get; set; }
	[Export] public PackedScene CursedAppleScene { get; set; }

	private Timer _spawnTimer;
	private Label _scoreLabel;
	private Label _highScoreLabel;
	private Label _livesLabel;

	private float _baseSpawnTime = 1.2f;
	private float _minSpawnTime = 0.25f;
	private float _minFallSpeed = 250.0f;
	private float _maxFallSpeedBonus = 700.0f;

	public override void _Ready()
	{
		_spawnTimer = GetNode<Timer>("SpawnTimer");
		_scoreLabel = GetNodeOrNull<Label>("CanvasLayer/ScoreLabel");
		_highScoreLabel = GetNodeOrNull<Label>("CanvasLayer/HighScoreLabel");
		_livesLabel = GetNodeOrNull<Label>("CanvasLayer/LivesLabel");

		var menuHighScore = GetNodeOrNull<Label>("CanvasLayer/StartMenu/VBoxContainer/HighScoreLabel");

		GetTree().Paused = true;
		var startMenu = GetNode<Control>("CanvasLayer/StartMenu");
		startMenu.Visible = true;

		if (menuHighScore != null)
		{
			menuHighScore.Text = $"Melhor Pontuação: {GameManager.Instance.HighScore}";
		}

		_spawnTimer.Stop();
		_spawnTimer.Timeout -= OnSpawnTimerTimeout;
		_spawnTimer.Timeout += OnSpawnTimerTimeout;

		GameManager.Instance.ScoreChanged -= OnScoreChanged;
		GameManager.Instance.LivesChanged -= OnLivesChanged;
		GameManager.Instance.GameOver -= OnGameOver;

		GameManager.Instance.ScoreChanged += OnScoreChanged;
		GameManager.Instance.LivesChanged += OnLivesChanged;
		GameManager.Instance.GameOver += OnGameOver;

		UpdateUI();
	}

	private void OnStartButtonPressed()
	{
		GetTree().Paused = false;
		GetNode<Control>("CanvasLayer/StartMenu").Visible = false;
		_spawnTimer.Start(_baseSpawnTime);
	}

	public override void _Process(double delta)
	{
		if (!GetTree().Paused)
		{
			UpdateUI();
			
			float score = (float)GameManager.Instance.Score;
			
			float difficultyFactor = score / 50f; 
			_spawnTimer.WaitTime = Mathf.Max(_minSpawnTime, _baseSpawnTime - (difficultyFactor * 0.15f));
		}
	}

	private void UpdateUI()
	{
		if (_scoreLabel != null) _scoreLabel.Text = $"Score: {GameManager.Instance.Score}";
		if (_highScoreLabel != null) _highScoreLabel.Text = $"Best: {GameManager.Instance.HighScore}";

		if (_livesLabel != null)
		{
			int currentLives = GameManager.Instance.Lives;
			string hearts = "";
			for (int i = 0; i < currentLives; i++) hearts += "❤️";
			_livesLabel.Text = hearts;
		}
	}

	private void OnSpawnTimerTimeout()
	{
		if (GetTree().Paused) return;

		GD.Randomize();

		PackedScene sceneToSpawn = (GD.Randf() > 0.4f) ? CursedAppleScene : GoodFoodScene;

		if (sceneToSpawn != null)
		{
			var instance = sceneToSpawn.Instantiate<FallingItem>();
			
			float speedFactor = (GameManager.Instance.Score / 50f) * 40.0f;
			instance.FallSpeed = _minFallSpeed + Mathf.Min(speedFactor, _maxFallSpeedBonus);

			float screenWidth = (float)ProjectSettings.GetSetting("display/window/size/viewport_width");
			if (screenWidth <= 0) screenWidth = 360;

			float randomX = (float)GD.RandRange(50, screenWidth - 50);

			instance.Position = new Vector2(randomX, -50);
			AddChild(instance);
		}
	}

	private void OnScoreChanged(int amount)
	{
		GetNodeOrNull<AudioStreamPlayer>("SfxGood")?.Play();
		GetNodeOrNull<Player>("Player")?.PlayFeedback(true);
	}

	private void OnLivesChanged(int currentLives)
	{
		if (currentLives > 0)
		{
			GetNodeOrNull<AudioStreamPlayer>("SfxBad")?.Play();
			GetNodeOrNull<Player>("Player")?.PlayFeedback(false);
		}
	}

	private async void OnGameOver()
	{
		GetNodeOrNull<AudioStreamPlayer>("SfxGameOver")?.Play();
		UpdateUI();
		await Task.Delay(100);
		GetTree().Paused = true;
		var screen = GetNodeOrNull<Control>("CanvasLayer/GameOverScreen");
		if (screen != null) screen.Visible = true;
	}

	private void OnRestartButtonPressed()
	{
		GameManager.Instance.ScoreChanged -= OnScoreChanged;
		GameManager.Instance.LivesChanged -= OnLivesChanged;
		GameManager.Instance.GameOver -= OnGameOver;

		GameManager.Instance.ResetGame();
		GetTree().Paused = false;
		GetTree().ReloadCurrentScene();
	}
}
