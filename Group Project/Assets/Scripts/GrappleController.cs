using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleController : MonoBehaviour
{
    public float maxDistance = 10f; // Maximum distance that grapple can reach
    public float minDistance = 5f;  // Minimum distance that grapple can reach
    public float step = .2f;        // Step for grapple retraction
    public LayerMask mask;          // Mask to tell what the grapple can attach to
    public LineRenderer line;       // Line renderer for texture of grapple
    public Transform aimContainer;
    public Transform aimTransform;
    public GameObject hook;

    //private Rigidbody2D rb2d;       // Player's rigid body 2D
    private GameObject player;        // Player holding object
    private PickupController pickup;  // Pickup controller for this object
    private Vector3 hookStart;
    private float hookAngle;

    DistanceJoint2D joint;          // Distance join 2D used for the grapple
    Vector3 targetPosition;         // Position player is aiming for. 
    RaycastHit2D hit;               // Raycast of object hit by grapple
    Vector2 connectPoint;

    // Start is called before the first frame update
    void Start()
    {
        pickup = GetComponent<PickupController>();  // Get pickup controller
        line.enabled = false;   // Disable the line renderer

        hookStart = hook.transform.localPosition;   // get starting position of hook
    }

    // Update is called once per frame
    void Update()
    {
        if (pickup.isEquipped)   // Check if equiped by a player
        {
            if(joint == null)
            {
                joint = GetComponent<PickupController>().player.GetComponent<DistanceJoint2D>();
                joint.enabled = false;  // Disable the joint at start
            }

            // Get target position for grapple
            float horizontal = Input.GetAxis("Look_Horizontal_P1");
            float vertical = Input.GetAxis("Look_Vertical_P1");
            //Debug.Log("X: " + horizontal.ToString() + "Y: " + vertical.ToString());

            float angle = 0f;

            if (horizontal == 0f && vertical == 0f)
            {
                // If no joystick input default to straight ahead
                angle = -90f;
            }
            else
            {
                // Get the angle
                angle = (-Mathf.Atan2(horizontal, vertical) * Mathf.Rad2Deg) - 90f;
            }

            //Debug.Log("Angle: " + angle.ToString());

            // Update the aim containers rotation to move the aim target
            aimContainer.eulerAngles = new Vector3(0, 0, angle);

            if (joint.distance > minDistance)    // Restract if not at minimum distance
            {
                joint.distance -= (step * Time.deltaTime); // Retract by step
            }
            else
            {
                Debug.Log("Breakign grapple");
                joint.enabled = false;  // Disable the joint
                line.enabled = false;   // Disable the line renderer
                hook.transform.parent = this.transform; // Set the hook back to child
                hook.transform.localPosition = hookStart;   // Put the hook back on the gun
                hook.transform.eulerAngles = new Vector3(0, 0, -90f);   // Set its rotation back
            }

            if (Input.GetButtonDown("Fire_P1"))
            {
                Debug.Log("Fire button pressed");
                if(!joint.enabled)
                {
                    Debug.Log("Raycasting");
                    // Raycast to get target position
                    hit = Physics2D.Raycast(transform.position, aimTransform.position - transform.position, maxDistance, mask);

                    // Check for raycasting hit
                    if (hit.collider != null && hit.collider.gameObject.GetComponent<Rigidbody2D>() != null)
                    {
                        Debug.Log("Hit something");
                        if (joint.enabled == false)
                        {
                            //rb2d.velocity = Vector2.zero;
                        }
                        joint.enabled = true;
                        joint.connectedBody = hit.collider.gameObject.GetComponent<Rigidbody2D>();

                        // Get the poiny on the hit object and scale by object scale
                        connectPoint = hit.point - new Vector2(hit.collider.transform.position.x, hit.collider.transform.position.y);
                        connectPoint.x = connectPoint.x / hit.collider.transform.localScale.x;
                        connectPoint.y = connectPoint.y / hit.collider.transform.localScale.y;
                        joint.connectedAnchor = connectPoint;

                        // get distance to point
                        joint.distance = Vector2.Distance(transform.position, hit.point);

                        // Set the hook to that point with correct angle
                        hook.transform.parent = null;
                        hook.transform.position = joint.connectedBody.transform.TransformPoint(joint.connectedAnchor);
                        hookAngle = -Mathf.Atan2(transform.position.x - connectPoint.x, transform.position.y - connectPoint.y) * Mathf.Rad2Deg;
                        hook.transform.eulerAngles = new Vector3(0, 0, angle);

                        // Set the line rederer
                        line.enabled = true;
                        line.SetPosition(0, transform.position);
                        line.SetPosition(1, joint.connectedBody.transform.TransformPoint(joint.connectedAnchor));
                    }
                }
                else
                {
                    // Update the line rederer and hook angle
                    line.SetPosition(0, transform.position);
                    hookAngle = -Mathf.Atan2(transform.position.x - connectPoint.x, transform.position.y - connectPoint.y) * Mathf.Rad2Deg;
                    hook.transform.eulerAngles = new Vector3(0, 0, angle);
                }
            }

            // Button released. Grapple over
            if (Input.GetButtonUp("Fire_P1"))
            {
                Debug.Log("Button Released");
                joint.enabled = false;
                line.enabled = false;
                hook.transform.parent = this.transform;
                hook.transform.localPosition = hookStart;
                hook.transform.eulerAngles = new Vector3(0, 0, -90f);
            }
        }
        else
        {
            Debug.Log("Weapon no longer equipped");
            if(joint != null)
            {
                joint.enabled = false;
                line.enabled = false;
                joint = null;
                hook.transform.parent = this.transform;
                hook.transform.localPosition = hookStart;
                hook.transform.eulerAngles = new Vector3(0, 0, -90f);
            }
        }
        
    }
}

