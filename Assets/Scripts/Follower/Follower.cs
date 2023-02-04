using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour {
    [SerializeField] public Collider2D col;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool spriteLookingLeft;

    private void OnCollisionEnter2D(Collision2D collision) {
        var interactable = collision.gameObject.GetComponent<FollowerInteractable>();
        if (interactable) {
            FindObjectOfType<FollowerManager>().Interact(interactable);
        }
    }

    public void UpdateVelocity(Vector3 velocity) {
        rb.velocity = velocity;
        anim.SetFloat("Velocity", velocity.magnitude);

        spriteRenderer.flipX = (spriteLookingLeft && velocity.x > 0) || (!spriteLookingLeft && velocity.x < 0);
    }
}
