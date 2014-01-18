using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	public GameObject main;                     // Main menu group
	public GameObject gameBoard;				// The gameboard group
	public AudioClip sfx;

	enum GameState { MainMenu, Playing, Paused };
	GameState gameState = GameState.MainMenu;
	
	Ray ray;
    RaycastHit2D hit;                           // The raycast to detect the target item
    Vector3 inputPos;
	
	// Update is called once per frame
	void Update () {
		// TODO use a GUI element instead of 3D collision detection
        ray = Camera.main.ScreenPointToRay(GetInputPosition());
		hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
	
		if (HasInput() && hit.collider != null) {
			audio.PlayOneShot(sfx);
			switch (gameState) {
		    	case (GameState.MainMenu):
		    		if (hit.transform.name == "Play") {
						Debug.Log("Playing");
                		StartCoroutine(PlayPressed(hit.transform));
					} 
					break;
				case (GameState.Paused):
					if (hit.transform.name == "backButton") {
						Debug.Log("Backing");
						StartCoroutine(BackPressed(hit.transform));
					}
					break;
				case (GameState.Playing):
					Debug.Log("Game on!!");
					break;
			}
		}

		// This also fires for Android's back button.
		// http://answers.unity3d.com/questions/369198/how-to-exit-application-in-android-on-back-button.html
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit(); 
		}
	}

	public void QuitToTitle() {
		Debug.Log("Quit game");
		gameState = GameState.MainMenu;
		gameBoard.SetActive(false);
		main.SetActive(true);
	}
	
	//Called when the play button is pressed
    IEnumerator PlayPressed(Transform button)
	{
		StartCoroutine(Animate(button, 0.1f, 0.2f));
		yield return new WaitForSeconds(0.3f);
		// Reset all fruits
		foreach (Transform t in gameBoard.transform) {
			if (t.tag == "SpawnPoint") {
				t.GetComponent<SpawnScript>().Restart();
			}
		}
		gameState = GameState.Playing;
        main.SetActive(false);
		gameBoard.SetActive(true);
    }

	//Called when the back button is pressed
	IEnumerator BackPressed(Transform button)
	{
		StartCoroutine(Animate(button, 0.1f, 0.2f));
		yield return new WaitForSeconds(0.3f);
		gameState = GameState.MainMenu;
		gameBoard.SetActive(false);
		main.SetActive(true);
	}
	
    //Returns true if there is an active input
    bool HasInput()
    {
		return Input.GetMouseButtonDown(0);
    }
    //Returns true, if the input was released
    bool InputReleased()
    {
        return Input.GetMouseButtonUp(0);
    }
    //Returns the input position
    Vector3 GetInputPosition()
    {
        return Input.mousePosition;
    }

    //Animates a button
    IEnumerator Animate(Transform button, float scaleFactor, float time)
    {
        Vector3 originalScale = button.localScale;

        float rate = 1.0f / time;
        float t = 0.0f;

        float d = 0;

        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            button.localScale = originalScale + (originalScale * (scaleFactor * Mathf.Sin(d * Mathf.Deg2Rad)));

            d = 180 * t;
            yield return new WaitForEndOfFrame();
        }

        button.localScale = originalScale;
    }

}
