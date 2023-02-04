using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerPickup : FollowerInteractable {
    public override IEnumerator Interact(FollowerManager manager) {
        yield return new WaitForSeconds(1);

    }

    public void MoveTo(Vector3 target) {

    }
}
