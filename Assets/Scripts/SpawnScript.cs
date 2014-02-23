using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This script generates the game pieces and detects matches (from input) 


namespace FrenzyGames.FruitGame {
	public class SpawnScript : MonoBehaviour {
		public Transform[] FruitPrefabList;  
		public Vector3 BoardSize;
		
		//private Transform SpawnPoint; 
		private Transform Floor; 
		public AudioClip match;
		public AudioClip noMatch;
		// tweak this until things look centered-ish
		private const float spawnOffsetX = -1.0f;
		// This should be a little bigger than the Collision2D.Size.X of the prefab fruits.
		// TODO: figure out how to reference that value directly somehow
		private const float sizeX = 1.6f;
		// This should be the exact value of the Collision2D.Size.Y of the prefab fruits
		private const float sizeY = 1.5f;

		private HashSet<string> fruitTags;

		// Publish an event when blocks are matched.
		// http://www.codeproject.com/Articles/11541/The-Simplest-C-Events-Example-Imaginable
		public event MatchHandler MatchEvent = delegate {};
		public delegate void MatchHandler(int removed);

		// prevent npe on the first restart. http://stackoverflow.com/a/340618
		public event RestartHandler RestartEvent = delegate {};
		public delegate void RestartHandler();

		// Called at the startup of the app
		void Start() {
			if (fruitTags == null) {
				fruitTags = new HashSet<string>();
				foreach (Transform fruit in FruitPrefabList) {
					fruitTags.Add(fruit.tag);
				}
			}

			//SpawnPoint = GameObject.Find("SpawnPoint").transform;
			Floor = GameObject.Find("Floor").transform;
		}

		public void Restart() {
			ClearBoard();
			SpawnFruitStartup();
			RestartEvent();
			// StartCoroutine(SpawnEvent());
		}
		public void ClearBoard() {
			foreach (Transform fruit in IterateFruit()) {
				DestroyObject(fruit.gameObject);
			}
		}
		void SpawnFruitStartup() {
			// 2D array (effectively) - i used to have a specific array defined, which we may have to bring back
			for (int x=0; x<BoardSize.x; x++) {
				for (int y=0; y<BoardSize.y; y++) {
					// this is the phsycial spawn point in game, relative to the SpawnPoint object
					Debug.Log(" Startup tpx: " + transform.position.x + " SpawnOffsetX: " + spawnOffsetX + " x: " + x + " sizex: " + sizeX);
					Vector3 spawnVector3 = new Vector3(transform.position.x + x * sizeX + spawnOffsetX,transform.position.y + y - 10, 0);
					// Actual gamepiece 
					Transform newCell;
					// pick a random piece 
		 			newCell = (Transform)Instantiate(FruitPrefabList[Random.Range (0, FruitPrefabList.Length)], spawnVector3, Quaternion.identity);
					newCell.parent = transform;
				}
			}
		}	
		public void ReSpawnFruit(float spawnPosition) {
			Debug.Log("ReSpwaning Fruit");
			// Y offset prevents most overlap weirdness from pushing things horizontally
			Vector3 reSpawnVector3 = new Vector3(spawnPosition, transform.position.y + Random.Range(0f, 10f), 0);
			Transform newCell;
			newCell = (Transform)Instantiate(FruitPrefabList[Random.Range (0, FruitPrefabList.Length)], reSpawnVector3, Quaternion.identity);
			newCell.parent = transform;
		}

		public int FindFruitColumn(float x) {
			return Mathf.RoundToInt((x - transform.position.x - spawnOffsetX) / sizeX);
		}
		private float RoundFruitColumn(float x) {
			return FindFruitColumn(x) * sizeX + transform.position.x + spawnOffsetX;
		}
		public int FindFruitRow(float y) {
			return Mathf.RoundToInt((y - Floor.position.y - sizeY/2) / sizeY);
		}

		public Transform[,] BuildMatcherGrid() {
			Transform[,] ret = new Transform[(int)BoardSize.x, (int)BoardSize.y];
			foreach (Transform fruit in transform) {
				var x = FindFruitColumn(fruit.position.x);
				var y = FindFruitRow(fruit.position.y);
				// This may be false for non-fruit, say, the floor
				if (0 <= x && x < BoardSize.x && 0 <= y && y < BoardSize.y) {
					ret[x, y] = fruit;
				}
			}
			Debug.Log(ret.Length + " fruits on the board");
			return ret;
		}

		bool IsFruit(Transform thing) {
			return fruitTags.Contains(thing.tag);
		}
		IEnumerable<Transform> IterateFruit() {
			foreach (Transform child in transform) {
				if (IsFruit(child)) {
					yield return child;
				}
			}
		}

		void FixedUpdate() {
			// Hack to stop horizontal drift of fruits. 
			// We should really put them all in a 2d array and update the array as we go, 
			// but that's a lot of work to change at this point, and it's hard to give fruits
			// their pretty falling behavior if we do that.
			foreach (Transform child in transform) {
				var p = child.localPosition;
				p.x = RoundFruitColumn(p.x);
				child.localPosition = p;
			}

			if (Input.GetMouseButtonDown(0)) {
				Debug.Log("Mouse Button Down");

				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
				
				Debug.Log("Hit: " + hit.transform);
				
				if (hit.collider != null) {
					Debug.Log("RayCast2D: " + hit.transform.tag);
					if (hit.transform.tag == "floor") {
						Debug.Log("Hit the floor");
						return;	
					}

					var x = FindFruitColumn(hit.transform.position.x);
					var y = FindFruitRow(hit.transform.position.y);
					Debug.Log(string.Format("grid: {0},{1} tag: {2}", x, y, hit.transform.tag));

					// Grab the attached component
					// hit is the actual gameobject that was clicked in gameplay
					CellScript cScript = hit.transform.gameObject.GetComponent<CellScript>();
					int matched = cScript.DetectMatches();
					// includes the clicked piece, so minimum match size is 2
					if (matched >= 2) {
						audio.PlayOneShot(match);
						cScript.DestroyMatches();
						// Publish an event when blocks are matched.
						MatchEvent(matched);
					} 
					else {
						audio.PlayOneShot(noMatch);
					}
				} else {
					Debug.Log("collider null: " + hit.transform);
				}	
			}
		}
	}
}