using UnityEngine;
using System.Collections;

namespace FrenzyGames.FruitGame {
	public class Score : MonoBehaviour {
		public int score { get; private set; }
		public GUISkin skin;
		public GameObject popupPrefab;

		void Start () {
			score = 0;
			// Listen for matched blocks.
			// http://docs.unity3d.com/412/Documentation/ScriptReference/index.Accessing_Other_Components.html
			var spawn = GetComponent<SpawnScript>();
			spawn.MatchEvent += OnMatch;
			spawn.RestartEvent += OnRestart;
		}

		private void OnMatch(MatchData match) {
			int scoreDelta = (match.NumRemoved - 1) * 100;
			score += scoreDelta;

			var popup = Instantiate(popupPrefab) as GameObject;
			var data = popup.GetComponent<ScorePopup>();
			data.PositionTouched = Camera.main.WorldToScreenPoint(match.Touched.position);
			Debug.Log (match.Touched.localPosition + "; " + match.Touched.position);
			data.Score = scoreDelta;
			popup.SetActive(true);
		}
		
		private void OnRestart() {
			score = 0;
		}

		public string formattedScore {
			get {
				// This would be better internationalization, but it broke around the time we started using
				// GUI skins and I'm not sure why.
				return string.Format("{0:N0}", score);
				//return string.Format("{0:N0}", score);
			}
		}

		public void OnGUI() {
			GUI.skin = skin;
			GUI.Label(new Rect (Screen.width * 0.7f, 0f, Screen.width * 0.3f, 60f), formattedScore, "score");
		}
	}
}