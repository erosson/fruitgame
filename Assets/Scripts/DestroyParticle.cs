using UnityEngine;
using System.Collections;

public class DestroyParticle : MonoBehaviour {
	void Start () {
		if (gameObject.particleSystem) {
	    	GameObject.Destroy(gameObject, gameObject.particleSystem.duration + gameObject.particleSystem.startLifetime);
	    }
	}
}