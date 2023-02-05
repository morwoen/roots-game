using CooldownManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class FollowerManager : MonoBehaviour {
    public enum FollowerState {
        AtPlayer,
        Returning,
        Charging,
        Interacting,
    }

    public delegate void OnFollowerChange();
    public event OnFollowerChange onFollowerChange;

    [Header("Movement Speeds")]
    [SerializeField] private float atPlayerSpeed = 1;
    [SerializeField] private float returningSpeed = 1;
    [SerializeField] private float chargingSpeed = 1;

    [Header("Positioning")]
    [SerializeField] private float[] distances;
    [SerializeField] private int[] numberOfFollowers;
    [SerializeField] private float[] angleOffsets;
    [SerializeField] private float directedDistanceFromPlayer = 1;

    [Header("Charging")]
    [SerializeField] private float chargeDuration;
    [SerializeField] private float[] chargeDistances;
    [SerializeField] private Transform chargeTelegraph;
    private Vector3 chargeDirection;
    private Cooldown chargeTimer;
    private List<Vector3> chargePositionOffsets;

    [Header("Interacting")]
    [SerializeField] private float spinSpeed = 1;
    [SerializeField] private Transform movementTelegraph;
    [SerializeField] private float maxMovementDistance = 3;
    [SerializeField] private float maxRangeFromPlayer = 5;
    private FollowerInteractable interactable;
    private float[] interactDistances;
    private float spinOffset = 0;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip chargeClip;
    [SerializeField] private AudioClip returnClip;

    [Header("Other")]
    [SerializeField] private PlayerInputController input;
    [SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private Animator anim;
    [SerializeField] private Follower followerPrefab;

    private List<Follower> followers;
    private List<Vector3> basePositionOffsets;
    private FollowerState state = FollowerState.AtPlayer;

    public int ActiveFollowers {
        get {
            return followers == null ? 0 : followers.Count;
        }
    }

    private void OnEnable() {
        followers = FindObjectsOfType<Follower>().ToList();
        onFollowerChange?.Invoke();
        basePositionOffsets = GetLayeredPositionsAround(Vector3.zero, distances, numberOfFollowers, angleOffsets);
        movementTelegraph.gameObject.SetActive(false);
    }

    private void OnDisable() {
        chargeTimer?.Stop();
    }

    private void Update() {
        if (state == FollowerState.AtPlayer) {
            // Move telegraph
            var angle = Mathf.Atan2(input.LookDirection.y, input.LookDirection.x) * Mathf.Rad2Deg;
            chargeTelegraph.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            // Move followers
            var pos = transform.position + input.LookDirection.To2DV3() * directedDistanceFromPlayer;
            for (int i = 0; i < followers.Count; i++) {
                var destination = pos + basePositionOffsets[i % basePositionOffsets.Count];
                var direction = destination - followers[i].transform.position;
                followers[i].UpdateVelocity(atPlayerSpeed * (direction.magnitude > .1 ? direction.normalized : direction));
            }

            if (input.Attack) {
                anim.SetTrigger("Attack");
                input.AttackPerformed();
                SwitchState(FollowerState.Charging);
                chargeDirection = input.LookDirection.To2DV3();
                var point = transform.position + chargeDirection * (chargingSpeed * chargeDuration);
                chargePositionOffsets = GetLayeredPositionsAround(point, chargeDistances, numberOfFollowers, angleOffsets);
                chargeTimer = Cooldown.Wait(chargeDuration).OnComplete(() => {
                    SwitchState(FollowerState.Returning);
                });
            }
        } else if (state == FollowerState.Returning) {
            var pos = transform.position + input.LookDirection.To2DV3() * directedDistanceFromPlayer;

            for (int i = 0; i < followers.Count; i++) {
                var destination = pos + basePositionOffsets[i % basePositionOffsets.Count];
                var direction = destination - followers[i].transform.position;
                followers[i].UpdateVelocity(returningSpeed * (direction.magnitude > 1 ? direction.normalized : direction));
            }
        } else if (state == FollowerState.Charging) {
            for (int i = 0; i < followers.Count; i++) {
                var direction = chargePositionOffsets[i] - followers[i].transform.position;
                followers[i].UpdateVelocity(returningSpeed * (direction.magnitude > 1 ? direction.normalized : direction));
            }
        } else if (state == FollowerState.Interacting) {
            if (!interactable) {
                SwitchState(FollowerState.Returning);
                return;
            }

            if (interactable.shouldSpin) {
                spinOffset = (spinOffset + spinSpeed * Time.deltaTime) % 360;
            }
            var pos = GetLayeredPositionsAround(interactable.transform.position, interactDistances, numberOfFollowers, angleOffsets, spinOffset);

            var easeInAtMagnetude = interactable is FollowerPickup ? .1f : 1;

            for (int i = 0; i < followers.Count; i++) {
                var direction = pos[i] - followers[i].transform.position;
                followers[i].UpdateVelocity(atPlayerSpeed * (direction.magnitude > easeInAtMagnetude ? direction.normalized : direction));
            }

            if (interactable is FollowerPickup) {
                var inter = (FollowerPickup)interactable;
                if (Vector3.Distance(transform.position, interactable.transform.position) > maxRangeFromPlayer) {
                    inter.Cancel();
                }

                if (input.LookDirection.magnitude == 0) {
                    movementTelegraph.gameObject.SetActive(false);
                } else {
                    movementTelegraph.gameObject.SetActive(!inter.Moving);
                    movementTelegraph.position = transform.position + input.LookDirection.To2DV3() * maxMovementDistance;

                    if (input.Attack) {
                        input.AttackPerformed();
                        inter.MoveTo(movementTelegraph.position);
                    }
                }
            }
        }
    }

    private void SwitchState(FollowerState newState) {
        switch (newState) {
            case FollowerState.Returning:
                audioSource.PlayOneShot(returnClip);

                foreach (var follower in followers) {
                    follower.col.enabled = false;
                }

                chargeTimer = Cooldown.Wait(chargeDuration).OnComplete(() => {
                    SwitchState(FollowerState.AtPlayer);
                });

                break;
            case FollowerState.AtPlayer:
                foreach (var follower in followers) {
                    follower.col.enabled = true;
                }
                break;
            case FollowerState.Charging:
                audioSource.PlayOneShot(chargeClip);
                break;
        }

        chargeTelegraph.gameObject.SetActive(newState == FollowerState.AtPlayer);

        state = newState;
    }

    private List<Vector3> GetLayeredPositionsAround(Vector3 center, float[] distances, int[] positions, float[] angleOffsets, float additionalOffset = 0) {
        List<Vector3> pos = new();
        for (int i = 0; i < distances.Length; i++) {
            pos.AddRange(GetPositionsAround(center, distances[i], positions[i], angleOffsets[i] + additionalOffset));
        }
        return pos;
    }

    private List<Vector3> GetPositionsAround(Vector3 center, float distance, int positions, float angleOffset) {
        List<Vector3> pos = new();
        for (int i = 0; i < positions; i++) {
            var angle = angleOffset + i * (360f / positions);
            Vector3 dir = Quaternion.Euler(new Vector3(0, 0, angle)) * Vector3.right;
            pos.Add(center + dir * distance);
        }
        return pos;
    }

    private void OnDrawGizmos() {
        Handles.Label(transform.position, state.ToString());
    }

    public void Interact(FollowerInteractable interactable) {
        if (state != FollowerState.Charging) return;
        chargeTimer?.Stop();
        this.interactable = interactable;
        interactDistances = chargeDistances.Select(d => d + interactable.distance).ToArray();

        if (interactable.shouldSpin) {
            foreach (var follower in followers) {
                follower.col.enabled = false;
            }
        }

        SwitchState(FollowerState.Interacting);

        StartCoroutine(ExecuteInteraction(interactable));
    }

    private IEnumerator ExecuteInteraction(FollowerInteractable interactable) {
        yield return interactable.Interact(this);

        movementTelegraph.gameObject.SetActive(false);

        SwitchState(FollowerState.Returning);
    }

    public void Spin() {
        spinOffset = (spinOffset + 100) % 360;
    }

    public void SpawnFollower(Vector3 position) {
        var follower = Instantiate(followerPrefab, position, Quaternion.identity);
        Cooldown.Wait(1).Always(() => {
            followers.Add(follower);
            onFollowerChange?.Invoke();
        });
    }

    public void TeleportFollowers() {
        for (int i = 0; i < followers.Count; i++) {
            var destination = transform.position + basePositionOffsets[i % basePositionOffsets.Count];
            followers[i].transform.position = destination;
        }
    }
}
