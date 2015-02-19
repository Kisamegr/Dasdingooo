using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour {

	public float maxHp;
	private float hp;

	// Use this for initialization
	void Start () {

		hp = maxHp;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player")
			Application.LoadLevel(Application.loadedLevel);
	}
}
