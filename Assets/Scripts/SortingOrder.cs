using UnityEngine;
using System.Collections;

public class SortingOrder : MonoBehaviour {
	void Start () {
		particleSystem.renderer.sortingLayerName = "GUI";
		particleSystem.renderer.sortingOrder = 2;
	}
}
