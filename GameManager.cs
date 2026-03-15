using Godot;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }

	[Signal] public delegate void ScoreChangedEventHandler(int amount);
	[Signal] public delegate void LivesChangedEventHandler(int currentLives);
	[Signal] public delegate void GameOverEventHandler();

	public int Score = 0;
	public int HighScore = 0;
	public int Lives = 5;

	private const string SavePath = "user://savegame.cfg";

	public override void _Ready()
	{
		Instance = this;
		LoadHighScore();
	}

	public void AddScore(int amount)
	{
		if (Lives <= 0) return;

		if (amount > 0)
		{
			Score += amount;
			if (Score > HighScore)
			{
				HighScore = Score;
				SaveHighScore();
			}
			EmitSignal(SignalName.ScoreChanged, amount);
		}
		else
		{
			LoseLife();
		}
	}

	public void ResetGame()
	{
		Score = 0;
		Lives = 5;
	}

	public void LoseLife()
	{
		if (Lives <= 0) return;
		Lives--;
		EmitSignal(SignalName.LivesChanged, Lives);
		if (Lives <= 0) EmitSignal(SignalName.GameOver);
	}

	private void SaveHighScore()
	{
		var config = new ConfigFile();
		config.SetValue("Player", "BestScore", HighScore);
		config.Save(SavePath);
	}

	private void LoadHighScore()
	{
		var config = new ConfigFile();
		Error err = config.Load(SavePath);

		if (err == Error.Ok)
		{
			HighScore = (int)config.GetValue("Player", "BestScore", 0);
		}
	}
}
