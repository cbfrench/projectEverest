using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//

[RequireComponent(typeof(BoxCollider2D))]
public class Stickman_MovementController : MonoBehaviour
{
    BoxCollider2D myCollider;

    RaycastOrigins raycastOrigins;
    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
    const float skinWidth = .015f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;
    float horizontalRaySpacing;
    float verticalRaySpacing;

    public LayerMask collisionMask; //What do we want to collide with?
    public LayerMask collisionMask_Below; //What do we want to collide with below us?
    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;
        public int faceDirection; //1 = right, -1 = left

        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
    }
    public CollisionInfo collisions;


    void Start()
    {
        myCollider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();

        collisions.faceDirection = 1; //initialize as facing right
    }

    void Update()
    {
        for (int i = 0; i < verticalRayCount; i++)
        {
            Debug.DrawRay(raycastOrigins.bottomLeft + Vector2.right * verticalRaySpacing * i, Vector2.up * -5, Color.red);
        }
        for (int i = 0; i < horizontalRayCount; i++)
        {
            Debug.DrawRay(raycastOrigins.bottomLeft + Vector2.up * horizontalRaySpacing * i, Vector2.right * -5, Color.red);
        }
    }

    void UpdateRaycastOrigins()
    {
        Bounds bounds = myCollider.bounds;
        //make raycast skindeep on all sides
        //negative to go in, 2 because top/bottom, right/left
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);


    }

    void CalculateRaySpacing()
    {
        Bounds bounds = myCollider.bounds;
        //make raycast skindeep on all sides
        //negative to go in, 2 because top/bottom, right/left
        bounds.Expand(skinWidth * -2);

        //require min of 2 horiz ntal and 2 vertical rays,
        //guarenteeing we always  have 1 in each corner
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        //NOTE- remember horizontal rays on on the y-axis (like y=1 is a horizontal line)
        //and vertical rays on the x-axis (graphing  x=4 is a vertical line)
        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);

    }

    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();
        collisions.Reset();

        if (velocity.x != 0)
        {
            collisions.faceDirection = (int)Mathf.Sign(velocity.x);
        }
        //for the purposes of wallsliding, we want to check horizontal collisions
        //EVEN if we're not moving side to side
        horizontalCollisions(ref velocity);


        if (velocity.y != 0)
        {
            verticalCollisions(ref velocity); //update velocity ('ref' makes it updated)
        }

        //after we've applied all our collisions, actually move our position
        transform.Translate(velocity);
    }

    //ref: updates velocity for the calling method as well (creates a side-effect)
    //think of it like velocity = verticalCollisions(velocity)
    void verticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y); //up = +1, down = -1
        float rayLength = Mathf.Abs(velocity.y) + skinWidth; //unsigned y velocity

        LayerMask mask;

        //Special case: only turn on collisions with "Platforms" that are below us
        if (directionY == -1)
        {
            mask = collisionMask_Below;
        }
        else
        {
            mask = collisionMask;
        }

        for (int i = 0; i < verticalRayCount; i++)
        {
            //start from the bottom if we're going down, from the top otherwise
            Vector2 rayOrigin = (directionY == -1.0) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;

            //we want to cast our rays from where we will be after moving side to side, 
            //therefore, + velocity.x
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);

            //when we draw our ray, we want to see if we've "hit something (vertically)"
            //We'll define something as "hit" if its in the direction we're moving (directionY)
            //AND marked as something we can hit (collisionMask) 
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, mask);

            //draw our ray
            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                //update collision info
                collisions.below = (directionY == -1);
                collisions.above = (directionY == 1);

                //we need to get the old "velocity.y" from when we defined rayLength,
                //therefore, to get velocity.y from a ray we need to:
                //subract skinWidth
                //hit.distance is always positive, so we need to reapply directionY
                velocity.y = (hit.distance - skinWidth) * directionY;

                //NOTE: we change the rayLength to hit distance once we hit something,
                //so that when later rays are drawn, we can hit anything further away.
                //This guarentees we collide with the closest object
                rayLength = hit.distance;
            }
        }
    }

    //same as verticalCollisions, but for horizontal collisions
    void horizontalCollisions(ref Vector3 velocity)
    {
        float directionX = collisions.faceDirection; //right = +1, left = -1
        float rayLength = Mathf.Abs(velocity.x) + skinWidth; //unsigned x velocity

        //ONLY KEEP THIS IS IF YOU WANT WALLSLIDING W/O DIRECTIONAL INPUT
        //(WOULD HAVE TO REMOVE OTHER THINGS AS WELL...) 
        if (Mathf.Abs(velocity.x) < skinWidth)
        {
            //ray length can be particularly small if our horizontal movement
            //is particularly slow
            rayLength = skinWidth * 2; //just needs to be a bit bigger than skinWidth

        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            //start rays depending on what direction we're going
            Vector2 rayOrigin = (directionX == -1.0) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;

            rayOrigin += Vector2.up * (horizontalRaySpacing * i);

            //We'll define something as "hit" if its in the direction we're moving (directionX)
            //AND marked as something we can hit (collisionMask) 
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            //draw our ray
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                //update collision info
                collisions.left = (directionX == -1);
                collisions.right = (directionX == 1);

                //we need to get the old "velocity.x" from when we defined rayLength,
                //therefore, to get velocity.x from a ray we need to:
                //subract skinWidth
                //hit.distance is always positive, so we need to reapply directionX
                velocity.x = (hit.distance - skinWidth) * directionX;

                //NOTE: we change the rayLength to hit distance once we hit something,
                //so that when later rays are drawn, we can hit anything further away.
                //This guarentees we collide with the closest object
                rayLength = hit.distance;
            }
        }
    }

}
