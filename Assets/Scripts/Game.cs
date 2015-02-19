using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Game : MonoBehaviour {



	public GameObject groundPrefab;
	public GameObject ceilingPrefab;

	public float yMax;
	public float yMin;
	public float cameraHeight;

	public float score;
	public Text scoreText;

	private Vector3 startingPos;


	private float groundWidth;
	private Transform lastGround;
	private Transform lastCeiling;

	private Transform camTrans;
	private Transform player;
	private Transform cleaner;
	
	private Queue groundQueue;
	private Queue ceilingQueue;

	void Start () {
		groundQueue = new Queue();
		ceilingQueue = new Queue();

		camTrans =  GameObject.FindGameObjectWithTag("MainCamera").transform;
		player = GameObject.FindGameObjectWithTag("Player").transform;
		cleaner = camTrans.FindChild("Cleaner");

		Vector3 cameraZero = camTrans.camera.ViewportToWorldPoint(new Vector3(0,0,0));
		Vector3 cameraTop = camTrans.camera.ViewportToWorldPoint(new Vector3(0,1,0));
		cameraHeight = cameraTop.y - cameraZero.y;

		yMax = cameraTop.y;
		yMin = yMax - 40;

		//Debug.Log(cameraTop);

		groundWidth = groundPrefab.renderer.bounds.size.x;

		GameObject ground=null,ceiling=null;

		for(int i=0 ; i<8 ; i++) {
			Vector3 pos = new Vector3(cameraZero.x + groundWidth * i,yMin,0);
			ground = (GameObject) GameObject.Instantiate(groundPrefab,pos,Quaternion.identity);
			groundQueue.Enqueue(ground.transform);

			Vector3 pos2 = new Vector3(cameraZero.x + groundWidth * i, yMax,0);
			ceiling = (GameObject) GameObject.Instantiate(ceilingPrefab,pos2,Quaternion.identity);
			ceilingQueue.Enqueue(ceiling.transform);

		}

		lastGround = ground.transform;
		lastCeiling = ceiling.transform;

		startingPos = player.position;
	}


	
	void Update () {
		float yCamera = Mathf.Clamp(player.position.y,yMin + cameraHeight/2 - 1, yMax - cameraHeight/2 + 1);
		camTrans.position = new Vector3(player.position.x+4, yCamera , camTrans.position.z);





	}

	void FixedUpdate() {


		score = player.position.x - startingPos.x;
		scoreText.text = "" + ((int) score);



		if(((Transform)groundQueue.Peek()).position.x < cleaner.position.x) {

			Transform ground = (Transform) groundQueue.Dequeue();
			groundQueue.Enqueue(ground);
			ground.position = new Vector3(lastGround.position.x + groundWidth, ground.position.y, 0);

			lastGround = ground;

		}

		if(((Transform)ceilingQueue.Peek()).position.x < cleaner.position.x) {
			
			Transform ceiling = (Transform) ceilingQueue.Dequeue();
			ceilingQueue.Enqueue(ceiling);
			ceiling.position = new Vector3(lastCeiling.position.x + groundWidth, ceiling.position.y, 0);
			
			lastCeiling = ceiling;
			
		}
	}
}
