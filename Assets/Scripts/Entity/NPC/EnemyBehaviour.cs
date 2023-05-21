﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform boundaryCheck = null;
    [SerializeField] 
    private LayerMask whatIsGround;         // A mask determining what is ground to the character
    const float groundedRadius = .2f;       // Radius of the overlap circle to determine if grounded

    private Rigidbody2D rigidbody2D;

    [SerializeField]
    private float moveSpeed;
    [Range(0, .3f)][SerializeField] 
    private float movementSmoothing = .05f; // How much to smooth out the movement
    private Vector3 velocity = Vector3.zero;
    private bool facingRight = true;

    private void Awake() {
        rigidbody2D = GetComponent<Rigidbody2D>();
        boundaryCheck.transform.localPosition = new Vector3(boundaryCheck.transform.localPosition.x + moveSpeed * movementSmoothing / 2f,
                                                            boundaryCheck.transform.localPosition.y, 
                                                            boundaryCheck.transform.localPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
        CheckForFlip();
        DoMovement();
    }

    private void DoMovement() {
        float movementDirection = facingRight == true ? 1f : -1f;
        float move = moveSpeed * movementDirection;

        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(move, rigidbody2D.velocity.y);
        // And then smoothing it out and applying it to the character
        rigidbody2D.velocity = Vector3.SmoothDamp(rigidbody2D.velocity, targetVelocity, ref velocity, movementSmoothing);
    }

    private void CheckForFlip() {
        bool isGrounded = false;

        // The enemy is grounded if a circlecast to the boundary check position hits anything designated as ground
        Collider2D[] colliders = Physics2D.OverlapCircleAll(boundaryCheck.position, groundedRadius, whatIsGround);
        for (int i = 0; i < colliders.Length; i++) {
            if (colliders[i].gameObject != gameObject) {
                isGrounded = true;
            }
        }

        if (!isGrounded) {
            Flip();
        }
    }

    private void Flip() {
        // Switch the way the entity is labelled as facing.
        facingRight = !facingRight;

        // Multiply the entity's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision != null) {
            if (collision.gameObject.CompareTag("Player")) {
                SceneManager.LoadScene("Battle Scene");
            }
        }
    }
}