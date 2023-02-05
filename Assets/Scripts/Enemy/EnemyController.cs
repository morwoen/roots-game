using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CooldownManagement;

public class EnemyController : MonoBehaviour {
    public enum EnemyState {
        Idle,
        Chasing,
        Attacking,
    }

    [SerializeField] private float speed = 5;
    [SerializeField] private float idleSpeed = 2;
    [SerializeField] private int damage = 5;
    [SerializeField] private float disengageDistance = 5;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float attackCooldown = 1;
    [SerializeField] private float attackPrep = .5f;
    [SerializeField] private float attackDuration = 1;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool spriteLookingLeft;
    [SerializeField] private Animator anim;
    [SerializeField] private AudioSource audioSource;


    private Vector3 origin;
    private EnemyState state = EnemyState.Idle;
    private PlayerHealth player;
    private Vector3 target;
    private Cooldown cd;
    private bool hit = false;

    private void OnEnable() {
        origin = transform.position;
    }

    private void OnDisable() {
        cd?.Stop();
    }

    private void Update() {
        if (state == EnemyState.Attacking) {
            rb.velocity = speed * target.normalized;
            spriteRenderer.flipX = (spriteLookingLeft && rb.velocity.x > 0) || (!spriteLookingLeft && rb.velocity.x < 0);
            anim.SetFloat("Velocity", rb.velocity.magnitude);
        } else {
            rb.velocity = Vector3.zero;
        }
    }

    private void SwitchState(EnemyState newState) {
        if (newState == EnemyState.Chasing) {
            hit = false;
            cd = Cooldown.Wait(attackCooldown).OnComplete(() => {
                target = player.transform.position - transform.position;
                cd = Cooldown.Wait(attackPrep).OnComplete(() => {
                    anim.SetTrigger("Attack");
                    audioSource.Play();
                    SwitchState(EnemyState.Attacking);
                });
            });
        } else if (newState == EnemyState.Attacking) {
            cd = Cooldown.Wait(attackDuration).OnComplete(() => {
                SwitchState(EnemyState.Chasing);
            });
        }

        state = newState;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        var hp = collision.gameObject.GetComponent<PlayerHealth>();
        if (hp) {
            player = hp;
            SwitchState(EnemyState.Chasing);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        var hp = collision.gameObject.GetComponent<PlayerHealth>();
        if (hp) {
            cd?.Stop();
            SwitchState(EnemyState.Idle);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        var hp = collision.gameObject.GetComponent<PlayerHealth>();
        if (hp && !hit && state == EnemyState.Attacking) {
            hit = true;
            hp.Damage(damage);
        }
    }
}
