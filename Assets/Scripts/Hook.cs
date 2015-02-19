using UnityEngine;
using System.Collections;

public class Hook : MonoBehaviour
{
    //
    public GameObject player;
    //
    public float hookAngle;
    //
    public float hookForce;
    //
    public float extraForce = 2f;
    //
    private Vector3 hookVector;
    //
    private float hookLength;
    //
    private Vector2 extraForceDirection;
    //
    private bool hit;
    //
    private Vector3 hitPoint;
    //
    LineRenderer line;

    // Use this for initialization
    void Start()
    {
        hit = false;
        hookLength = 0;
        line = (LineRenderer)gameObject.GetComponent<LineRenderer>();

        hookVector = new Vector3(Mathf.Cos(Mathf.Deg2Rad * hookAngle), Mathf.Sin(Mathf.Deg2Rad * hookAngle), 0);

        transform.RotateAround(transform.position, Vector3.forward, hookAngle);


        float forceAngle = hookAngle - 90;
        forceAngle *= Mathf.Deg2Rad;

        extraForceDirection = new Vector2(Mathf.Cos(forceAngle), Mathf.Sin(forceAngle));
        extraForceDirection.Normalize();

	}

    // Update is called once per frame
    void Update()
    {
        if (hit)
        {
			transform.position = hitPoint;
			//transform.RotateAround(transform.position,transform.forward,Vector3.Angle() );
			transform.rotation = Quaternion.Euler( new Vector3(0,0,Vector3.Angle(hitPoint - player.transform.position,Vector3.right)));
			//Debug.Log(transform.rotation);


        }
        else
        {

            hookLength += Time.deltaTime * hookForce;

            transform.position = player.transform.position + hookLength * hookVector;
        }

        line.SetPosition(0, player.transform.position);
        
        //Leptomereies
        if (hit)
        {
            line.SetPosition(1, hitPoint);
        }
        else
        {
            line.SetPosition(1, transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Ceiling")
        {
            if (hit) return;
            hit = true;


            rigidbody2D.velocity = Vector2.zero;

            DistanceJoint2D joint = (DistanceJoint2D)player.GetComponent<DistanceJoint2D>();


            float xColPoint = (transform.position.x - other.transform.position.x);// / other.renderer.bounds.size.x;

            //To katw shmeio tou ceiling
            float yColPoint = -other.renderer.bounds.extents.y;// -transform.renderer.bounds.max.y;

            Vector2 connectedAnchor = new Vector2(xColPoint, yColPoint);

            //Vector2 anchor = Vector2.zero;

            joint.enabled = true;
            joint.connectedBody = other.rigidbody2D;
            //joint.anchor = anchor;
            joint.connectedAnchor = connectedAnchor;
            //joint.maxDistanceOnly = false;
            joint.distance = Vector2.Distance(player.transform.position, transform.position);


                
            hitPoint = new Vector3(other.transform.position.x + xColPoint, other.transform.position.y + yColPoint, 0);

            player.rigidbody2D.AddForce(extraForceDirection * extraForce, ForceMode2D.Impulse);
			player.GetComponent<Player>().hooked = true;

            player.rigidbody2D.gravityScale = 0;

        }

    }

}
