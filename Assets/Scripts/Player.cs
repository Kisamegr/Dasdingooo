using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

    public GameObject hookPrefab;


    public bool shotHook;
	public bool hooked;
	public bool jumped;

	public float maxSpeed;
	public float jumpForce;
	public float hookDelay;

    private GameObject hook;
    private DistanceJoint2D hookJoint;

	private float lastHookTime;

    // Use this for initialization
    void Start()
    {
        shotHook = false;
		jumped = false;
        hookJoint = (DistanceJoint2D)gameObject.GetComponent<DistanceJoint2D>();
        hookJoint.enabled = false;


		//rigidbody2D.centerOfMass = new Vector2(0,-renderer.bounds.extents.y);

		lastHookTime = -100;
    }

    // Update is called once per frame
    void Update()
    {
		//Debug.Log(rigidbody2D.velocity.y);
		if (!hooked)
		{
			if (rigidbody2D.velocity.y > 0 && rigidbody2D.gravityScale != 2)
			{
				rigidbody2D.gravityScale = 2;
			}
			if (rigidbody2D.velocity.y <= 0 && rigidbody2D.gravityScale != 1)
			{
				rigidbody2D.gravityScale = 1;
			}

			float z = transform.rotation.eulerAngles.z;

			if(z > 180)
				z = z - 360;

			rigidbody2D.AddTorque(-z * 0.02f,ForceMode2D.Force);
		}
		else {
			jumped = false;
		}

        //if(Input.GetAxis("Fire1") > 0 ) {
        if (Input.GetKeyDown(KeyCode.X))
        {
			shootHook ();
        }
        else if (Input.GetKeyUp(KeyCode.X))
        {
			cancelHook();
        }

        float moveForce = 4f;

        if (Input.GetAxis("Horizontal") > 0)
        {
            transform.rigidbody2D.AddForce(Vector2.right * moveForce, ForceMode2D.Force);
            //transform.position = new Vector3(transform.position.x + 0.2f, transform.position.y,0);
        }
        if (Input.GetAxis("Horizontal") < 0)
        {
            transform.rigidbody2D.AddForce(-Vector2.right * moveForce, ForceMode2D.Force);
            //transform.position = new Vector3(transform.position.x - 0.2f, transform.position.y,0);
        }

		if(rigidbody2D.velocity.x > maxSpeed) {
			rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x * 0.95f,rigidbody2D.velocity.y);
		}


		if(Input.GetKeyDown(KeyCode.Z) && !jumped && !hooked) {
			rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x,0);
			rigidbody2D.AddForce(Vector3.up*jumpForce , ForceMode2D.Impulse);
			jumped = true;
		}

    }

    void FixedUpdate()
    {
    }



    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Ceiling")
			cancelHook();

		if(other.collider.tag == "Ground")
			Application.LoadLevel(Application.loadedLevel);
        
    }

	void shootHook()
	{
		if (!shotHook && Time.time - lastHookTime > hookDelay)
		{
			shotHook = true;
			hook = (GameObject)GameObject.Instantiate(hookPrefab, transform.position, Quaternion.identity);
			hook.transform.parent = transform;
			hook.GetComponent<Hook>().player = gameObject;

			lastHookTime = Time.time;
		}
	}
	
	void cancelHook()
	{
		if (shotHook)
		{
			shotHook = false;
			hooked = false;
			rigidbody2D.gravityScale = 1;
			Destroy(hook);
			hookJoint.enabled = false;
		}
	}
}
