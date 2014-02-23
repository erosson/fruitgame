using UnityEngine;
using System.Collections;
using FrenzyGames.FruitGame;


namespace FrenzyGames.FruitGame {
	public class Pause : MonoBehaviour {
		public Texture2D pauseIcon;
		public Texture2D unpauseIcon;
		public AudioClip pauseSound;
		public AudioClip unpauseSound;
		public AudioClip gameOverSound;
		public GameObject mainMenu;

		private enum State {Unpaused, Paused, QuitDialog, GameOver};
		private State state = State.Unpaused;

		public event GameOverHandler GameOverEvent = delegate {};
		public delegate void GameOverHandler(Score score);

		// http://docs.unity3d.com/Documentation/Components/GUIScriptingGuide.html
		void OnGUI() {
			// http://answers.unity3d.com/questions/46158/how-to-create-a-transparent-button.html
			Rect buttonRect = new Rect(0, 0, Screen.width * 0.07f, Screen.width * 0.07f);
			GUI.skin.button.fontSize = 48;
			GUI.skin.label.fontSize = 48;
			// currently paused, hits the unpause button
			if (state == State.Paused) {
				GUI.backgroundColor = Color.clear;
				if (GUI.Button(buttonRect, unpauseIcon)) {
					Unpause();
				}
				GUI.backgroundColor = Color.black;
				GUILayout.BeginArea(new Rect(Screen.width * 0.3f, Screen.height * 0.3f, Screen.width * 0.4f, Screen.height * 0.4f));
				if (GUILayout.Button("Unpause")) {
					Unpause();
				}
				if (GUILayout.Button("Quit to Title")) {
					state = State.QuitDialog;
					audio.PlayOneShot(unpauseSound);
				}
				if (Debug.isDebugBuild) {
					GUI.backgroundColor = GUI.color = Color.red;
					if (GUILayout.Button("Force Game Over")) {
						state = State.GameOver;
						// TODO inspector score reference
						GameOverEvent(GetComponent<Score>());
						audio.PlayOneShot(gameOverSound);
					}
				}
				GUILayout.EndArea();
			}
			// currently unpaused, hits the pause button
			else if (state == State.Unpaused) {
				GUI.backgroundColor = Color.clear;
				if (GUI.Button(buttonRect, pauseIcon)) {
					Pause_();
				}
			}
			else if (state == State.QuitDialog) {
				GUI.backgroundColor = Color.black;
				GUILayout.BeginArea(new Rect(Screen.width * 0.3f, Screen.height * 0.3f, Screen.width * 0.4f, Screen.height * 0.4f));
				if (GUILayout.Button("Oops, Don't Quit")) {
					state = State.Paused;
					audio.PlayOneShot(unpauseSound);
				}
				if (GUILayout.Button("Really Quit")) {
					QuitToTitle();
				}
				GUILayout.EndArea();
			}
			else if (state == State.GameOver) {
				// http://docs.unity3d.com/Documentation/ScriptReference/GUI.Window.html
				GUI.backgroundColor = Color.black;
				GUI.skin.window.fontSize = 48;
				GUILayout.Window(0, new Rect(Screen.width * 0.3f, Screen.height * 0.3f, Screen.width * 0.4f, Screen.height * 0.4f), GameOverWindow, "Game Over!");
			}
			// else assert false
		}

		private void GameOverWindow(int id) {
			// centered. http://forum.unity3d.com/threads/15455-Center-GUI-Component
			GUILayout.Label(""); // quick spacing hack
			GUILayout.Label("Score: " + GetComponent<Score>().formattedScore);
			if (GUILayout.Button("Return to Title")) {
				QuitToTitle();
			}
		}

		// "member names cannot be the same as their enclosing type." wtf, C#?
		private void Pause_() {
			state = State.Paused;
			audio.PlayOneShot(pauseSound);
			// http://answers.unity3d.com/questions/7544/how-do-i-pause-my-game.html
			// Prevents fruits from falling, and also seems to prevent matching on click.
			Time.timeScale = 0;
		}

		private void Unpause() {
			state = State.Unpaused;
			audio.PlayOneShot(unpauseSound);
			Time.timeScale = 1;
		}

		private void QuitToTitle() {
			// Unpause to restore timescale first, else main menu buttons won't work.
			Unpause();
			mainMenu.GetComponent<MainMenu>().QuitToTitle();
		}

		void Update() {
			// This also fires for Android's back button.
			if (Input.GetKeyDown(KeyCode.Escape)) {
				// while playing, back button/escape pauses.
				if (state == State.Unpaused) {
					Pause_();
				}
				// while paused, back button/escape quits the game, returning to the title screen.
				// Show a confirmation dialog first.
				else if (state == State.Paused) {
					state = State.QuitDialog;
					audio.PlayOneShot(unpauseSound);
				}
				else if (state == State.QuitDialog) {
					QuitToTitle();
				}
				else if (state == State.GameOver) {
					QuitToTitle();
				}
				// else assert false
			}
		}
	}
}