using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerAttack : FollowerInteractable {
    [SerializeField] private bool yieldFollower = false;

    private int damage = 5;

    public override IEnumerator Interact(FollowerManager manager) {
        var health = GetComponent<EnemyHealth>();
        yield return new WaitForSeconds(.8f);

        manager.Spin();
        if (health.Damage(damage)) {
            if (yieldFollower) {
                manager.SpawnFollower(transform.position);
            }
            yield return new WaitForSeconds(.8f);
            yield break;
        }

        yield return new WaitForSeconds(1);

        manager.Spin();
        if (health.Damage(damage)) {
            if (yieldFollower) {
                manager.SpawnFollower(transform.position);
            }
            yield return new WaitForSeconds(.8f);
            yield break;
        }

        yield return new WaitForSeconds(1);

        manager.Spin();
        if (health.Damage(damage)) {
            if (yieldFollower) {
                manager.SpawnFollower(transform.position);
            }
            yield return new WaitForSeconds(.8f);
            yield break;
        }

        yield return new WaitForSeconds(.8f);
    }
}
