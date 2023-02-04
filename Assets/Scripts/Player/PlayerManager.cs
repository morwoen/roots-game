using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    [SerializeField] private float speed = 1;
    [SerializeField] private PlayerInputController input;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool spriteLookingLeft;

    private void FixedUpdate() {
        rb.velocity = speed * input.Movement;
        anim.SetFloat("Velocity", rb.velocity.magnitude);

        spriteRenderer.flipX = (spriteLookingLeft && rb.velocity.x > 0) || (!spriteLookingLeft && rb.velocity.x < 0);
    }
}
