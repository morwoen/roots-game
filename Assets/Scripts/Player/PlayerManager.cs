using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    [SerializeField] private float speed = 1;
    [SerializeField] private PlayerInputController input;
    [SerializeField] private Rigidbody2D rb;

    private void FixedUpdate() {
        rb.velocity = speed * input.Movement;
    }
}
