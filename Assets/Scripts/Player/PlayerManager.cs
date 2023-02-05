using CooldownManagement;
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
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip stepSound;
    [SerializeField] private float stepSoundDelay;
    private Cooldown step;

    private void FixedUpdate() {
        rb.velocity = speed * input.Movement;
        anim.SetFloat("Velocity", rb.velocity.magnitude);

        spriteRenderer.flipX = (spriteLookingLeft && rb.velocity.x > 0) || (!spriteLookingLeft && rb.velocity.x < 0);

        if (rb.velocity.magnitude > 0) {
            if (!step) {
                audioSource.PlayOneShot(stepSound, 0.5f);
                step = Cooldown.Wait(stepSoundDelay);
            }
        }
    }
}
