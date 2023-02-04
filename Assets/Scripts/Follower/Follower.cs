using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour {
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] public Collider2D col;

    private void OnCollisionEnter2D(Collision2D collision) {
        var interactable = collision.gameObject.GetComponent<FollowerInteractable>();
        if (interactable) {
            FindObjectOfType<FollowerManager>().Interact(interactable);
        }
    }
}
