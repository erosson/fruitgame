using UnityEngine;
using System.Collections;

namespace FrenzyGames.FruitGame {
	public class ScorePopup : MonoBehaviour {
		public float YAnimate = 0;
		public GUISkin Skin;
		public Vector3 PositionTouched;
		public int Score;
		
		public void Start() {
			Debug.Log("score popup: " + Score + "; "+PositionTouched);
		}
		
		public void OnGUI() {
			GUI.skin = Skin;
			GUI.Label(new Rect(PositionTouched.x, Screen.height - PositionTouched.y - YAnimate, 200, 100), string.Format("{0:N0}", Score));
		}
		
		private void EndAnimation() {
			Destroy(gameObject);
		}
	}
}
