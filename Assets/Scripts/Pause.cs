using UnityEngine;
using System.Collections;

public class Pause : MonoBehaviour {
	public Texture2D pauseIcon;
	public Texture2D unpauseIcon;
	public AudioClip pauseSound;
	public AudioClip unpauseSound;
	public GameObject mainMenu;

	private enum State {Unpaused, Paused, QuitDialog};
	private State state = State.Unpaused;
	
	// http://docs.unity3d.com/Documentation/Components/GUIScriptingGuide.html
	void OnGUI() {
		// http://answers.unity3d.com/questions/46158/how-to-create-a-transparent-button.html
		Rect buttonRect = new Rect(0, 0, 50, 50);
		// currently paused, hits the unpause button
		if (state == State.Paused) {
			GUI.backgroundColor = Color.clear;
			if (GUI.Button(buttonRect, unpauseIcon)) {
				Unpause();
			}
			GUI.backgroundColor = Color.black;
			var width=100;
			var height=80;
			GUI.Window(0, new Rect((Screen.width - width)/2, (Screen.height - height)/2, width, height), PauseWindow,
			           "Paused");
		}
		// currently unpaused, hits the pause button
		else if (state == State.Unpaused) {
			GUI.backgroundColor = Color.clear;
			if (GUI.Button(buttonRect, pauseIcon)) {
				Pause_();
			}
		}
		else if (state == State.QuitDialog) {
			// http://docs.unity3d.com/Documentation/ScriptReference/GUI.Window.html
			var width=160;
			var height=80;
			GUI.backgroundColor = Color.black;
			GUI.ModalWindow(0, new Rect((Screen.width - width)/2, (Screen.height - height)/2, width, height), QuitWindow,
			                "Quit and return to title?");
		}
		// else assert false
	}
	private void QuitWindow(int windowID) {
		if (GUI.Button(new Rect(10, 20, 140, 20), "Don't Quit")) {
			state = State.Paused;
			audio.PlayOneShot(unpauseSound);
		}
		if (GUI.Button(new Rect(10, 50, 140, 20), "Quit")) {
			QuitToTitle();
		}
	}
	private void PauseWindow(int windowID) {
		if (GUI.Button(new Rect(10, 20, 80, 20), "Resume")) {
			Unpause();
		}
		if (GUI.Button(new Rect(10, 50, 80, 20), "Quit")) {
			state = State.QuitDialog;
			audio.PlayOneShot(unpauseSound);
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
			// else assert false
		}
	}
}
