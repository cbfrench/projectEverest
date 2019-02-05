using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleController : MonoBehaviour
{

    public bool isEquiped = false;  // Stores boolean value of if grapple is equiped by a player
    public float maxDistance = 10f; // Maximum distance that grapple can reach
    public float minDistance = 5f;  // Minimum distance that grapple can reach
    public float step = .2f;        // Step for grapple retraction
    public LayerMask mask;          // Mask to tell what the grapple can attach to
    public LineRenderer line;       // Line renderer for texture of grapple

    //private Rigidbody2D rb2d;       // Player's rigid body 2D
    private GameObject player;      // Player holding object

    DistanceJoint2D joint;          // Distance join 2D used for the grapple
    Vector3 targetPosition;         // Position player is aiming for. 
    RaycastHit2D hit;               // Raycast of object hit by grapple

    // Start is called before the first frame update
    void Start()
    {
        joint = GetComponent<DistanceJoint2D>();    // Get the distance join
        joint.enabled = false;  // Disable the joint at start
        line.enabled = false;   // Disable the line renderer
        //rb2d = GetComponent<Rigidbody2D>(); // Get the rigid body of player
    }

    // Update is called once per frame
    void Update()
    {
        if (isEquiped)   // Check if equiped by a player
        {
            if (joint.distance > minDistance)    // Restract if not at minimum distance
            {
                joint.distance -= step; // Retract by step
            }
            else    // If grapple at minimum distance then break
            {
                joint.enabled = false;  // Disable the joint
                line.enabled = false;   // Disable the line renderer
            }

            if (Input.GetMouseButtonDown(0))
            {
                targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                targetPosition.z = 0;

                hit = Physics2D.Raycast(transform.position, targetPosition - transform.position, maxDistance, mask);

                if (hit.collider != null && hit.collider.gameObject.GetComponent<Rigidbody2D>() != null)
                {
                    if (joint.enabled == false)
                    {
                        //rb2d.velocity = Vector2.zero;
                    }
                    joint.enabled = true;
                    joint.connectedBody = hit.collider.gameObject.GetComponent<Rigidbody2D>();

                    Vector2 connectPoint = hit.point - new Vector2(hit.collider.transform.position.x, hit.collider.transform.position.y);
                    connectPoint.x = connectPoint.x / hit.collider.transform.localScale.x;
                    connectPoint.y = connectPoint.y / hit.collider.transform.localScale.y;
                    joint.connectedAnchor = connectPoint;

                    joint.distance = Vector2.Distance(transform.position, hit.point);

                    line.enabled = true;
                    line.SetPosition(0, transform.position);
                    //line.SetPosition(1, hit.collider.transform.position);
                    //line.SetPosition(1, connectPoint);
                    line.SetPosition(1, joint.connectedBody.transform.TransformPoint(joint.connectedAnchor));
                }

            }

            //line.SetPosition(1, joint.connectedBody.transform.TransformPoint(joint.connectedAnchor));

            if (Input.GetMouseButton(0))
            {
                line.SetPosition(0, transform.position);
            }

            if (Input.GetMouseButtonUp(0))
            {
                joint.enabled = false;
                line.enabled = false;
            }
        }
        
    }


    // Sets isEquipped value. Used by other scripts
    void SetEquipped(bool val)
    {
        isEquiped = val;
    }

    void SetPlayer(GameObject thePlayer)
    {
        player = thePlayer;
    }
}

