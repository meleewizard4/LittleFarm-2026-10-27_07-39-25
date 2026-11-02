using UnityEngine;

public class PlayerMovement_Mouse : MonoBehaviour
{
    public Rigidbody2D rb;
    public float moveSpeed = 5f;

    private Vector2 targetPosition;
    private Vector2 movement;
    private bool isMoving = false;

    public Animator animator;

    void Update()
    {
        if (Input.GetMouseButtonDown(button: 1))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isMoving = true;
        }
        
        if (isMoving == true)
        {
            movement = (targetPosition - rb.position).normalized;

            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", isMoving ? 1 : 0);
        }
    }


    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        if (Vector2.Distance(rb.position, targetPosition) <= 0.1f)
        {
            isMoving = false;
            movement = Vector2.zero;
            animator.SetFloat("Speed", 0);
        }
    }
}
