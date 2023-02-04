using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerPickup : FollowerInteractable {
    [SerializeField] private float speed = 2;

    public bool Moving { get; set; } = false;

    private bool destinationReached = false;
    private Vector3 target;

    public override IEnumerator Interact(FollowerManager manager) {
        while (!destinationReached) {
            yield return new WaitForEndOfFrame();
        }

        destinationReached = false;
    }

    public void MoveTo(Vector3 target) {
        Moving = true;
        this.target = target;
    }

    public void Cancel() {
        destinationReached = true;
        Moving = false;
    }

    private void Update() {
        if (!Moving) return;

        var direction = target - transform.position;
        transform.Translate(speed * Time.deltaTime * (direction.magnitude > 1 ? direction.normalized : direction));
        if (direction.magnitude < .1) {
            Moving = false;
            destinationReached = true;
        }
    }
}
