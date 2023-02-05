using CooldownManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour {
    [SerializeField] private float after = 1;

    private void Start() {
        Cooldown.Wait(after).OnComplete(() => Destroy(gameObject));
    }
}
