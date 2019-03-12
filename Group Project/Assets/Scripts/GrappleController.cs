using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrappleController : MonoBehaviour, WeaponScript
{
    public Text label;
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
    private Vector3 hookStart;
    private float hookAngle;
    private bool grappling;

    DistanceJoint2D joint;          // Distance join 2D used for the grapple
    Vector3 targetPosition;         // Position player is aiming for. 
    RaycastHit2D hit;               // Raycast of object hit by grapple
    Vector2 connectPoint;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize 
        player = null;

        // Set label text
        label.text = "Grapple Gun";
        label.gameObject.SetActive(true);

        line.enabled = false;   // Disable the line renderer
        hookStart = hook.transform.localPosition;   // get starting position of hook
        grappling = false;
    }

    // Called when a player picks up the weapon
    public void initWeaponUnique(GameObject player)
    {
        // Set the player reference
        this.player = player;
        label.gameObject.SetActive(false);

        // Get the distance joint of the player
        joint = player.gameObject.GetComponent<DistanceJoint2D>();
    }

    // Called when a player drops the weapon
    public void resetWeaponUnique(GameObject player)
    {
        // Set the player reference back to null on drop
        this.player = null;
        label.gameObject.SetActive(true);

        // Set the joint to null
        joint = null;
    }

    private void Update()
    {
        if (grappling)
        {
            if (joint.distance > minDistance)    // Restract if not at minimum distance
            {
                joint.distance -= (step * Time.deltaTime); // Retract by step
                line.SetPosition(0, transform.position);
                hookAngle = -Mathf.Atan2(transform.position.x - hook.transform.position.x, transform.position.y - hook.transform.position.y) * Mathf.Rad2Deg;
            }
            else
            {
                breakGrapple();
            }
        }
    }

    public void shoot()
    {
        grappling = true;

        if (!joint.enabled)
        {
            // Raycast to get target position
            hit = Physics2D.Raycast(transform.position, aimTransform.position - transform.position, maxDistance, mask);

            // Check for raycasting hit
            if (hit.collider != null && hit.collider.gameObject.GetComponent<Rigidbody2D>() != null)
            {
                // Damage the player
                if(hit.collider.gameObject.tag == "Player")
                {
                    hit.collider.gameObject.GetComponent<PlayerController>().receiveDamage(20);
                }

                //Debug.Log("Hit something");
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
                hook.transform.eulerAngles = new Vector3(0, 0, hookAngle);

                // Set the line rederer
                line.enabled = true;
                line.SetPosition(0, transform.position);
                line.SetPosition(1, joint.connectedBody.transform.TransformPoint(joint.connectedAnchor));
            }
        }
    }

    public void stop()
    {
        grappling = false;
        breakGrapple();
    }

    public void breakGrapple()
    {
        // Reset all grapple components
        joint.enabled = false;
        line.enabled = false;
        hook.transform.parent = this.transform;
        hook.transform.localPosition = hookStart;
        hook.transform.eulerAngles = new Vector3(0, 0, -90f);
    }
}
