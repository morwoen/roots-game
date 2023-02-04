using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerAttack : FollowerInteractable {
    private int damage = 5;

    public override IEnumerator Interact(FollowerManager manager) {
        var health = GetComponent<EnemyHealth>();
        yield return new WaitForSeconds(.8f);

        manager.Spin();
        if (health.Damage(damage)) {
            yield break;
        }

        yield return new WaitForSeconds(1);

        manager.Spin();
        if (health.Damage(damage)) {
            yield break;
        }

        yield return new WaitForSeconds(1);

        manager.Spin();
        if (health.Damage(damage)) {
            yield break;
        }

        yield return new WaitForSeconds(.8f);
    }
}
