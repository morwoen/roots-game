using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FollowerInteractable : MonoBehaviour {
    public bool shouldSpin = false;
    public float distance = 1;

    public abstract IEnumerator Interact(FollowerManager manager);
}
