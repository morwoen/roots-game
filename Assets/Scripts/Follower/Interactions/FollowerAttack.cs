using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerAttack : FollowerInteractable {
    public override IEnumerator Interact(FollowerManager manager) {
        
        yield return new WaitForSeconds(1);

    }
}
