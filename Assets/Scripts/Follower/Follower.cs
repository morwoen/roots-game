using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour {
    //public enum FollowerState {
    //    AtPlayer,
    //    Returning,
    //    Charging,
    //}

    //[SerializeField] private float atPlayerSpeed = 1;
    //[SerializeField] private float returningSpeed = 1;
    //[SerializeField] private float chargingSpeed = 1;

    [SerializeField] private PlayerManager player;
    [SerializeField] public Rigidbody2D rb;

    //private FollowerState state = FollowerState.Returning;

    //private void Update() {
    //    if (state == FollowerState.AtPlayer) {
    //        rb.velocity = atPlayerSpeed * (player.transform.position - transform.position).normalized;
    //    } else if (state == FollowerState.Returning) {
    //        rb.velocity = returningSpeed * (player.transform.position - transform.position).normalized;
    //    } else if (state == FollowerState.Charging) {
    //        //rb.velocity = returningSpeed * (player.transform.position - transform.position).normalized;
    //    }
    //}

}
