using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// This script is attached to every game piece object
// Keeps track of collisions as they enter/stay/exit


public class CellScript : MonoBehaviour {
	public Transform CollectParticlesPrefab;
	
	private GameObject SpawnPoint;
	
	// Use this for initialization
	void Start () {
		SpawnPoint = GameObject.Find("SpawnPoint");
	}
	// Update is called once per frame
	void Update () {
	
	}
	// callback functions - invoked by unity from the physics engine
	// TODO sounds
	void OnCollisionStay2D(Collision2D collision) { 
	}
	// same
	void OnCollisionExit2D(Collision2D collision) { 
	}

	// Returns a list of all fruits matching this one.
	private List<Transform> FindMatches() {
		var spawn = transform.parent.GetComponent<SpawnScript>();
		var grid = spawn.BuildMatcherGrid();
		var tags  = new List<string>();
		
		// iterate the array vertically, horizontally, and diagonally looking for matches.
		var matched = new List<Transform>();
		var matchChecks = new Stack<Transform>();
		matchChecks.Push(transform);
		while (matchChecks.Count != 0) {
			var checking = matchChecks.Pop();
			matched.Add(checking);
			var x = spawn.FindFruitColumn(checking.position.x);
			var y = spawn.FindFruitRow(checking.position.y);
			// All directions one square away from this one, including diagonally
			for (int dx = -1; dx <= 1; dx++) {
				for (int dy = -1; dy <= 1; dy++) {
					var x2 = x + dx;
					var y2 = y + dy;
					if (0 <= x2 && x2 < spawn.BoardSize.x && 0 <= y2 && y2 < spawn.BoardSize.y) {
						var candidate = grid[x2, y2];
						if (candidate) {
							tags.Add (candidate.tag);
							if (candidate.tag == checking.tag && !matched.Contains(candidate)) {
								// it's a new match! Check for more matches adjacent to this match.
								matchChecks.Push(candidate);
							}
						}
					}
				}
			}
		}
		return matched;
	}

	// here is the function we call from Spawnscript	
	// scan the array created by the above events
	// return a match number
	public int DetectMatches() {
		return FindMatches().Count;
	}
	public void DestroyMatches() { 
		foreach (Transform death in FindMatches()) { 
			if (CollectParticlesPrefab) {
				Instantiate(CollectParticlesPrefab, death.transform.position, Quaternion.identity);
			}
			Debug.Log("Matched: " + death.transform.position.x);
			SpawnPoint.GetComponent<SpawnScript>().ReSpawnFruit(death.transform.position.x);
			DestroyObject(death.gameObject);
		}
	}
}
