using UnityEngine;

public class PlayerWallLock : MonoBehaviour
{
    private bool isGrounded;
    private bool touchingLeftWall;
    private bool touchingRightWall;

    public bool IsGrounded => isGrounded;
    public bool TouchingLeftWall => touchingLeftWall;
    public bool TouchingRightWall => touchingRightWall;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        AnalyseCollision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        AnalyseCollision(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        
        isGrounded = false;
        touchingLeftWall = false;
        touchingRightWall = false;
    }

    private void AnalyseCollision(Collision2D collision)
    {
        bool foundGround = false;
        bool foundLeftWall = false;
        bool foundRightWall = false;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            Vector2 normal = contact.normal;

           
            if (normal.y > 0.5f)
            {
                foundGround = true;
            }

          
            if (normal.x > 0.8f)
            {
                foundLeftWall = true;
            }

          
            if (normal.x < -0.8f)
            {
                foundRightWall = true;
            }
        }

        isGrounded = foundGround;

        if (isGrounded)
        {
            touchingLeftWall = false;
            touchingRightWall = false;
        }
        else
        {
            touchingLeftWall = foundLeftWall;
            touchingRightWall = foundRightWall;
        }
    }

   
}