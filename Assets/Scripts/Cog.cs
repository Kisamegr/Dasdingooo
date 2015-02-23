using UnityEngine;
using System.Collections;

public class Cog : MonoBehaviour {
	
	public float rotationSpeed;
	
	private Vector3 rotVector;
	// Use this for initialization
	void Start () {
		
		rotVector = new Vector3(0,0,rotationSpeed);
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		transform.Rotate(rotVector);
	}
}
