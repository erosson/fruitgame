using UnityEngine;
using UnityEngine.SocialPlatforms;
using System.Collections;

public class AchievementPopup : MonoBehaviour {
	public float y = 0;
	public IAchievementDescription achievement = null;
	public GUIStyle style;

	public void Start() {
		Debug.Log("achievement popup: " + achievement.title);
	}

	public void OnGUI() {
		GUILayout.BeginArea(new Rect(Screen.width * 0.8f, Screen.height * (1 - y), Screen.width * 0.2f, Screen.height * 0.2f), style);
		GUILayout.BeginHorizontal("box");
		GUILayout.Box(achievement.image, GUILayout.Width(64), GUILayout.Height (64));
		GUILayout.BeginVertical();
		GUILayout.Box(achievement.title, style);
		var desc = achievement.achievedDescription != "" ? achievement.achievedDescription : achievement.unachievedDescription;
		GUILayout.Box(desc, style);
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	private void EndAnimation() {
		Debug.Log("achievement popdown: " + achievement.title);
		Destroy(gameObject);
	}
}
