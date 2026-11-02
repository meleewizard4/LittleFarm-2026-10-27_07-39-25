using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb; 
    public float moveSpeed = 5f;

    private Vector2 movement;

    public Animator animator;

    void Update()
    {
        movement.x = Input.GetAxisRaw(axisName: "Horizontal");
        movement.y = Input.GetAxisRaw(axisName: "Vertical");

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }


    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

}