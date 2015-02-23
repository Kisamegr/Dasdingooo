using UnityEngine;
using System.Collections;

public class Cannon : MonoBehaviour
{


    private bool rotationPhase;

    private bool forcePhase;

    //
    public float rotation;

    public float rotationSpeed;

    public float startAngle;

    public float endAngle;
    //
    private float force;

    public float maxForce;

    public float minForce;

    public float forceSpeed;

    //

    public GameObject player;


    

    // Use this for initialization
    void Start()
    {
        
        //player = GameObject.FindGameObjectWithTag("Player");
        player.transform.parent = transform;
        player.transform.localPosition = transform.FindChild("PlayerPosition").localPosition;
        
        
        player.transform.Rotate(0, 0, -90);
        player.rigidbody2D.gravityScale = 0;
        player.rigidbody2D.fixedAngle = true;
        player.GetComponent<Player>().inCannon = true;


        if (startAngle > endAngle)
        {
            float temp = startAngle;
            startAngle = endAngle;
            endAngle = temp;
        }
        rotation = startAngle;
        transform.rotation = Quaternion.Euler(0, 0, startAngle);
        rotationPhase = true;
        forcePhase = false;
        force = 0;
    }

    // Update is called once per frame
    void Update()
    {
        

        if (rotationPhase)
        {
            rotation += rotationSpeed * Time.deltaTime;

            if (rotation > endAngle)
            {
                transform.rotation = Quaternion.Euler(0, 0, endAngle);
                rotation = endAngle;
                rotationSpeed = -rotationSpeed;
            }

            if (rotation < startAngle)
            {
                transform.rotation = Quaternion.Euler(0, 0, startAngle);
                rotation = startAngle;
                rotationSpeed = -rotationSpeed;
            }

            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

            if(Input.GetKeyDown(KeyCode.Space)){
                rotationPhase = false;
                forcePhase = true;
                return;
            }
        }


        if (forcePhase)
        {
            //Debug.Log(force);
            force += forceSpeed * Time.deltaTime;
            if (force >= 1)
            {
                force = 1;
                forceSpeed = -forceSpeed;
            }

            if (force <= 0)
            {
                force = 0;
                forceSpeed = -forceSpeed;
            }
            Debug.Log(force);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                fire();
                forcePhase = false;
            }
            

        }
    }


    private void fire()
    {
        Vector2 cannonForceDir = new Vector2(Mathf.Cos(transform.rotation.eulerAngles.z * Mathf.Deg2Rad) ,Mathf.Sin(transform.rotation.eulerAngles.z * Mathf.Deg2Rad));

        float finalForce = minForce + force * (maxForce - minForce);

        player.GetComponent<Player>().fireFromCannon(cannonForceDir * finalForce);
        
    }


    public void OnDrawGizmosSelected()
    {
        float length = transform.FindChild("Cannon1_sprite").renderer.bounds.size.x;


        Vector3 point1 = transform.position + new Vector3(Mathf.Cos(startAngle * Mathf.Deg2Rad),Mathf.Sin(startAngle * Mathf.Deg2Rad),0)*length;

        Vector3 point2 = transform.position + new Vector3(Mathf.Cos(endAngle * Mathf.Deg2Rad),Mathf.Sin(endAngle * Mathf.Deg2Rad),0)*length;


        Gizmos.color = Color.blue;

        Gizmos.DrawLine(transform.position, point1);

        Gizmos.DrawLine(transform.position, point2);
    
    }





}
