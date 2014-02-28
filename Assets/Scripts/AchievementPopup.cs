using UnityEngine;
using UnityEngine.SocialPlatforms;
using System.Collections;

public class AchievementPopup : MonoBehaviour {
	public float y = 0;
	public IAchievementDescription achievement = null;
	public GUISkin skin;

	public void Start() {
		Debug.Log("achievement popup: " + achievement.title);
	}

	public void OnGUI() {
		GUI.skin = skin;
		GUILayout.BeginArea(new Rect(Screen.width * 0.7f, Screen.height * (1 - y), Screen.width * 0.3f, Screen.height * 0.15f));
		GUILayout.BeginHorizontal("box");
		var iconsize = Screen.height*0.15f;
		GUILayout.Label(achievement.image, GUILayout.Width(iconsize), GUILayout.Height (iconsize));
		GUILayout.BeginVertical();
		GUILayout.Label(achievement.title);
		var desc = achievement.achievedDescription != "" ? achievement.achievedDescription : achievement.unachievedDescription;
		GUILayout.Box(desc);
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	private void EndAnimation() {
		Debug.Log("achievement popdown: " + achievement.title);
		Destroy(gameObject);
	}
}
