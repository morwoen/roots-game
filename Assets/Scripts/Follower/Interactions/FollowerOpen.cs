using CooldownManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FollowerOpen : FollowerInteractable {
    [SerializeField] private Collider2D col;
    [SerializeField] private int followersRequired;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image lockImage;
    [SerializeField] private GameObject vines;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openClip;
    [SerializeField] private AudioClip notEnoughFollowersClip;

    private Cooldown colorReset;

    private void OnEnable() {
        text.SetText(followersRequired.ToString());
        if (followersRequired == 0) {
            Destroy(lockImage.gameObject);
            Destroy(text.gameObject);
        }
    }

    public override IEnumerator Interact(FollowerManager manager) {
        yield return new WaitForSeconds(1);

        if (manager.ActiveFollowers < followersRequired) {
            text.color = Color.red;
            lockImage.color = Color.red;

            colorReset = Cooldown.Wait(3).Always(() => {
                text.color = Color.white;
                lockImage.color = Color.white;
            });

            audioSource.PlayOneShot(notEnoughFollowersClip);

            yield break;
        }

        yield return new WaitForSeconds(1);

        colorReset?.Stop();
        if (lockImage) {
            Destroy(lockImage.gameObject);
        }
        if (text) {
            Destroy(text.gameObject);
        }
        Destroy(vines);
        audioSource.PlayOneShot(openClip);
        col.enabled = false;
    }

    private void Reset() {
        col = GetComponent<Collider2D>();
    }
}
