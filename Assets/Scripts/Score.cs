using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {
	public int score { get; private set; }
	public GUIStyle style;

	void Start () {
		score = 0;
		// Listen for matched blocks.
		// http://docs.unity3d.com/412/Documentation/ScriptReference/index.Accessing_Other_Components.html
		var spawn = GetComponent<SpawnScript>();
		spawn.MatchEvent += OnMatch;
		spawn.RestartEvent += OnRestart;
	}

	private void OnMatch(int removed) {
		int scoreDelta = (removed - 1) * 100;
		score += scoreDelta;
	}

	private void OnRestart() {
		score = 0;
	}

	public void OnGUI() {
		string scoreWithCommas = string.Format("{0:n0}", score);
		GUI.Label (new Rect (Screen.width - 100,0,100,50), scoreWithCommas, style);
	}
}
