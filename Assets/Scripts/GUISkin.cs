using UnityEngine;

public class GUISkin : MonoBehaviour {
	public UnityEngine.GUISkin skin;

	void OnGUI () {
		// Setting the skin can only be done from within onGUI, but I'd rather put it in start() so it only runs once
		GUI.skin = skin;
		gameObject.SetActive(false);
	}
}