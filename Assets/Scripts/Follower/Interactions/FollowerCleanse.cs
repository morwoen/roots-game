using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FollowerCleanse : FollowerInteractable {
    [SerializeField] private int followersRequired;

    private bool isCleansed = false;

    public override IEnumerator Interact(FollowerManager manager) {
        yield return new WaitForSeconds(1);

        if (isCleansed || manager.ActiveFollowers < followersRequired) {
            yield break;
        }

        yield return new WaitForSeconds(2);

        isCleansed = true;
    }

    private void OnDrawGizmos() {
        Handles.Label(transform.position, isCleansed ? "Cleansed" : "Corrupted");
    }
}
