using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

    public GameObject hookPrefab;
	public GameObject brokenPrefab;


    public bool shotHook;
	public bool hooked;
	public bool jumped;
	public bool startJump;


    public bool inCannon;
    public bool firedFromCannon;

	public float moveForce;
	public float maxSpeed;
	public float jumpForce;
	public float hookDelay;
	public float hookAngle;
	public float stabilizerForce;

    private GameObject hook;
    private DistanceJoint2D hookJoint;

	private Animator anim;

	private float lastHookTime;

	public bool running;
	public bool onAir;
	public bool facingRight;


	private bool hitCeiling;
	private float ceilingPenaltyStart;

	private float zeta;

    // Use this for initialization
    void Start()
    {
        shotHook = false;
		jumped = false;
        hookJoint = (DistanceJoint2D)gameObject.GetComponent<DistanceJoint2D>();
        hookJoint.enabled = false;
		anim = transform.GetChild(0).GetComponent<Animator>();


		//rigidbody2D.centerOfMass = new Vector2(0,-renderer.bounds.extents.y);

		lastHookTime = -100;
		running = false;
		onAir = true;
		facingRight = true;
    }

	IEnumerator GameOver() {

		cancelHook();

		transform.GetChild(0).renderer.enabled = false;
		gameObject.collider2D.enabled = false;

		GameObject broken = (GameObject) Instantiate(brokenPrefab,transform.position,Quaternion.identity);

		for(int i=0 ;i<broken.transform.childCount ; i++) {
			Transform child = (Transform) broken.transform.GetChild(i);

			child.rigidbody2D.velocity = rigidbody2D.velocity;

		}


		yield return new WaitForSeconds(3);

		Application.LoadLevel(Application.loadedLevel);
	}

    // Update is called once per frame
    void Update()
    {
	
        //An einai mesa sto kanoni min kaneis tpt
        if (inCannon)
        {
            return;
        }

        //An exei petaxtei apo to kanoni tote perimene mexri na arxisei na katevainei. Ka8ws anevainei min kaneis tpt
        if (firedFromCannon)
        {
            if (rigidbody2D.velocity.y > -0.1)
            {
				anim.SetBool("jump",false);
                return;
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                firedFromCannon = false;
				rigidbody2D.fixedAngle = false;

            }
        }



		running = false;
		startJump = false;
		zeta = transform.rotation.eulerAngles.z;

		if(zeta > 180)
			zeta = zeta - 360;

		//Debug.Log(zeta);

		// Stabilizer
		if(!shotHook && !hooked && !onAir) 
			rigidbody2D.AddTorque(-zeta * stabilizerForce,ForceMode2D.Force);

		if(onAir)
			rigidbody2D.AddTorque(-zeta * stabilizerForce/30 - 5,ForceMode2D.Force);

		if(shotHook)
			rigidbody2D.AddTorque(-zeta * stabilizerForce  -20,ForceMode2D.Force);

		
		if(hooked) {
			Vector3 hookVec = hook.transform.position - transform.position;
			float angle = Vector3.Angle(Vector3.up,hookVec);

			if(transform.position.x < hook.transform.position.x)
				angle *= -1;

			rigidbody2D.AddTorque(-zeta * stabilizerForce +  angle*2.5f,ForceMode2D.Force);


		}
	


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




		}
		else {
			jumped = false;
		}


		if( shotHook || hooked) {

			//Vector3 hh = hook.transform.position - transform.position;

			//rigidbody2D.AddTorque(-(zeta + 90 - hookAngle + Vector3.Angle(transform.up,hh)) * 0.04f,ForceMode2D.Force);

		}

        
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


		
		if(Input.GetKeyDown(KeyCode.Z) && !jumped && !hooked) {
			rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x,0);
			rigidbody2D.AddForce(Vector3.up*jumpForce , ForceMode2D.Impulse);
			jumped = true;
			startJump = true;
		}



        if (rigidbody2D.velocity.x > maxSpeed)
        {
            rigidbody2D.velocity = new Vector2(maxSpeed, rigidbody2D.velocity.y);
        }



		anim.SetBool("running",running);
		anim.SetBool("jump",startJump);
		anim.SetFloat("ySpeed",rigidbody2D.velocity.y);
		anim.SetBool("shotHook",shotHook);
		anim.SetBool("hooked",hooked);
		anim.SetBool("onAir",onAir);
		anim.SetBool("hitCeiling",hitCeiling);

		hitCeiling = false;
    }


    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Ceiling")
		{
			hitCeiling = true;
			ceilingPenaltyStart = Time.time;
			cancelHook();
		}

		if(other.collider.tag == "Ground")
			StartCoroutine(GameOver());

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
		if (!shotHook && Time.time - lastHookTime > hookDelay && Time.time - ceilingPenaltyStart > 1.5)
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



    public void fireFromCannon(Vector2 cannonForce)
    {
        transform.parent = null;
        //startJump = true;
        anim.SetBool("jump", true);
        rigidbody2D.AddForce(cannonForce, ForceMode2D.Impulse);
        if (rigidbody2D.velocity.y > 0)
        {
            rigidbody2D.gravityScale = 2;
        }
        inCannon = false;
        firedFromCannon = true;
    }

}
