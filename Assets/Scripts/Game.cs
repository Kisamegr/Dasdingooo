using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Game : MonoBehaviour {



	public GameObject groundPrefab;
	public GameObject ceilingPrefab;
	public GameObject[] platforms;

	public float yMax;
	public float yMin;
	public float cameraHeight;

	public float platformFreq;
	public float platformVar;
	private float lastPlatformTime;
	private float nextPlatformTime;
	private Transform platformSpawner;

	public float score;
	public Text scoreText;

	private Vector3 startingPos;


	private float groundWidth;
	private float ceilingWidth;
	private Transform lastGround;
	private Transform lastCeiling;

	public Transform camTrans;
	public Transform player;
	private Transform cleaner;
	
	private Queue groundQueue;
	private Queue ceilingQueue;
	private Queue platformQueue;

	void Start () {
		groundQueue = new Queue();
		ceilingQueue = new Queue();
		platformQueue = new Queue();

		//camTrans =  GameObject.FindGameObjectWithTag("MainCamera").transform;
		//player = GameObject.FindGameObjectWithTag("Player").transform;
		cleaner = camTrans.FindChild("Cleaner");
		platformSpawner = camTrans.FindChild("PlatformSpawner");

		Vector3 cameraZero = camTrans.camera.ViewportToWorldPoint(new Vector3(0,0,0));
		Vector3 cameraTop = camTrans.camera.ViewportToWorldPoint(new Vector3(0,1,0));
		cameraHeight = cameraTop.y - cameraZero.y;

		yMax = cameraTop.y;
		yMin = yMax - 40;

		lastPlatformTime = 0;
		nextPlatformTime = 0;

		//Debug.Log(cameraTop);

		groundWidth = groundPrefab.transform.GetChild(0).renderer.bounds.size.x -2;
		ceilingWidth = ceilingPrefab.renderer.bounds.size.x;
		//groundWidth = 10;

		GameObject ground=null,ceiling=null;

		bool reverse = false;

		for(int i=0 ; i<8 ; i++) {
			Vector3 pos = new Vector3(cameraZero.x + groundWidth * i,yMin,0);
			ground = (GameObject) GameObject.Instantiate(groundPrefab,pos,Quaternion.identity);

			Transform cog = ground.transform.GetChild(1);
			cog.localScale = new Vector3(groundWidth/cog.renderer.bounds.size.x,cog.localScale.y,cog.localScale.z);

			if(reverse) 
				ground.transform.GetChild(0).GetComponent<Cog>().rotationSpeed *= -1;

			reverse = !reverse;
			groundQueue.Enqueue(ground.transform);

			Vector3 pos2 = new Vector3(cameraZero.x + ceilingWidth * i, yMax,0);
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



		CreatePlatform();

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
			ceiling.position = new Vector3(lastCeiling.position.x + ceilingWidth, ceiling.position.y, 0);
			
			lastCeiling = ceiling;
			
		}

		if( platformQueue.Count > 0 && ((Transform)platformQueue.Peek()).position.x < cleaner.position.x) {
			
			Transform platform = (Transform) platformQueue.Dequeue();
		
			Destroy(platform.gameObject);
			
		}

	}

	public void CreatePlatform() {

		if(Time.time - lastPlatformTime > nextPlatformTime) {

			int index = Random.Range(0, platforms.GetLength(0));
			Vector3 pos = new Vector3(platformSpawner.position.x, Random.Range(yMin + 5,yMax - 5), 0);

			GameObject plat = (GameObject) Instantiate(platforms[index],pos,Quaternion.identity);


			platformQueue.Enqueue(plat.transform);

			nextPlatformTime =  Random.Range(-platformVar, platformVar) + platformFreq;
			lastPlatformTime = Time.time;
		}
	}
}
