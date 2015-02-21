using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

    public GameObject hookPrefab;


    public bool shotHook;
	public bool hooked;
	public bool jumped;
	public bool startJump;

	public float moveForce;
	public float maxSpeed;
	public float jumpForce;
	public float hookDelay;
	public float hookAngle;

    private GameObject hook;
    private DistanceJoint2D hookJoint;

	private Animator anim;

	private float lastHookTime;

	public bool running;
	public bool onAir;
	public bool facingRight;

	private float zeta;

    // Use this for initialization
    void Start()
    {
        shotHook = false;
		jumped = false;
        hookJoint = (DistanceJoint2D)gameObject.GetComponent<DistanceJoint2D>();
        hookJoint.enabled = false;
		anim = gameObject.GetComponent<Animator>();


		//rigidbody2D.centerOfMass = new Vector2(0,-renderer.bounds.extents.y);

		lastHookTime = -100;
		running = false;
		onAir = true;
		facingRight = true;
    }

    // Update is called once per frame
    void Update()
    {
		running = false;
		startJump = false;
		zeta = transform.rotation.eulerAngles.z;

		if(zeta > 180)
			zeta = zeta - 360;

		//Debug.Log(zeta);

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



			if(!shotHook)
				rigidbody2D.AddTorque(-zeta * 0.02f,ForceMode2D.Force);
		}
		else {
			jumped = false;
		}

		if( shotHook || hooked) {

			//Vector3 hh = hook.transform.position - transform.position;

			//rigidbody2D.AddTorque(-(zeta + 90 - hookAngle + Vector3.Angle(transform.up,hh)) * 0.04f,ForceMode2D.Force);

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


        if (Input.GetAxis("Horizontal") > 0)
        {
			running = true;
			if(!facingRight)
				Flip();
            transform.rigidbody2D.AddForce(Vector2.right * moveForce, ForceMode2D.Force);
            //transform.position = new Vector3(transform.position.x + 0.2f, transform.position.y,0);
        }
        if (Input.GetAxis("Horizontal") < 0)
        {
			running = true;
			if(facingRight)
				Flip();
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
			startJump = true;
		}


		anim.SetBool("running",running);
		anim.SetBool("jump",startJump);
		anim.SetFloat("ySpeed",rigidbody2D.velocity.y);
		anim.SetBool("shotHook",shotHook);
		anim.SetBool("hooked",hooked);
		anim.SetBool("onAir",onAir);

    }


    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Ceiling")
			cancelHook();

		if(other.collider.tag == "Ground")
			Application.LoadLevel(Application.loadedLevel);

		if(other.collider.tag == "Platform")
		{
			onAir = false;
			jumped = false;
		}
        
    }

	void OnCollisionExit2D(Collision2D coll) {
		if(coll.collider.tag == "Platform")
			onAir = true;
	}



	void shootHook()
	{
		if (!shotHook && Time.time - lastHookTime > hookDelay)
		{
			shotHook = true;
			hook = (GameObject)GameObject.Instantiate(hookPrefab, transform.position, Quaternion.identity);
			hook.transform.parent = transform;
			hook.GetComponent<Hook>().player = gameObject;
			hook.GetComponent<Hook>().hookAngle = hookAngle;


			lastHookTime = Time.time;
		}
	}
	
	void cancelHook()
	{
		if (shotHook || hooked)
		{
			shotHook = false;
			hooked = false;
			rigidbody2D.gravityScale = 1;
			Destroy(hook);
			hookJoint.enabled = false;
		}
	}

	void Flip()
	{
		// Switch the way the player is labelled as facing
		facingRight = !facingRight;
		
		// Multiply the player's x local scale by -1
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
