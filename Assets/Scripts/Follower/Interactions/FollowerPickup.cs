using CooldownManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FollowerPickup : FollowerInteractable {
    [SerializeField] private float speed = 2;
    [SerializeField] private int followersRequired;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image lockImage;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pickUpSound;
    [SerializeField] private AudioClip dropSound;
    [SerializeField] private AudioClip notEnoughFollowersClip;

    public bool Moving { get; set; } = false;

    private bool destinationReached = false;
    private Vector3 target;

    private Cooldown colorReset;

    private void OnEnable() {
        text.SetText(followersRequired.ToString());
        if (followersRequired == 0) {
            Destroy(lockImage.gameObject);
            Destroy(text.gameObject);
        }
    }

    public override IEnumerator Interact(FollowerManager manager) {
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

        colorReset?.Stop();
        if (lockImage) {
            Destroy(lockImage.gameObject);
        }
        if (text) {
            Destroy(text.gameObject);
        }

        audioSource.PlayOneShot(pickUpSound);
        while (!destinationReached) {
            yield return new WaitForEndOfFrame();
        }

        audioSource.PlayOneShot(dropSound);
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
