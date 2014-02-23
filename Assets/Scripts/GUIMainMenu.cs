using UnityEngine;
using UnityEngine.SocialPlatforms;
using System.Collections;
using System.Collections.Generic;

// TODO: delete mainmenu.cs in favor of this.
namespace FrenzyGames.FruitGame {
	public class GUIMainMenu : MonoBehaviour {
		public Leaderboard leaderboard;
		public SpawnScript spawn;
		public MainMenu mainMenu;
		public AudioClip sfx;
		public GUIStyle style;
		public int maxScoresShown;

		private enum State { Main, HighScore };
		private State state = State.Main;

		private IScore[] highScores = null;
		private Vector2 scrollPosition = Vector2.zero;

		void Start() {
			spawn.RestartEvent += OnRestart;
		}

		private string HighScoreText {
			get {
				if (highScores == null) {
					return "Loading...";
				}
				var ret = new List<string>();

				// limit number of scores shown. Negative or 0: no (practical) maximum
				var maxScoresShown = this.maxScoresShown > 0 ? this.maxScoresShown : long.MaxValue;

				for (var i=0; i < highScores.Length && i < maxScoresShown; i++) {
					var score = highScores[i];
					var rank = i+1;
					ret.Add(rank + ": " + score.formattedValue + " (" + score.date + ")"); 
				}
				return string.Join("\n", ret.ToArray());
			}
		}
		
		public void OnGUI() {
			// Never ever display any of this if the MainMenu, where state is currently tracked, thinks we're in the middle of the game.
			// TODO: delete MainMenu and maintain this state here, once this becomes the real MainMenu.
			if (mainMenu.gameState == MainMenu.GameState.MainMenu) {
				if (state == State.Main) {
					if (GUI.Button(new Rect(0, Screen.height * (1 - 0.12f), Screen.width * 0.4f, Screen.height * 0.12f), "High Score List")) {
						audio.PlayOneShot(sfx);
						scrollPosition = Vector2.zero;
						state = State.HighScore;
						leaderboard.LoadScores(scores => {
							this.highScores = scores;
						});
						// hack to break the "play" button while highscore gui is showing
						Time.timeScale = 0;
					}
				}
				else if (state == State.HighScore) {
					GUILayout.Label("High Scores", style);
					scrollPosition = GUILayout.BeginScrollView(
						scrollPosition, false, true, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height * 0.7f));
					GUILayout.Box(HighScoreText);
					GUILayout.EndScrollView();
					if (GUILayout.Button("Exit")) {
						audio.PlayOneShot(sfx);
						state = State.Main;
						// hack to break the "play" button while highscore gui is showing
						Time.timeScale = 1;
					}
				}
			}
		}

		private void OnRestart() {
			state = State.Main;
		}
	}
}