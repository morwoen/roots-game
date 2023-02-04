using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Linq;
using CooldownManagement;

public class PlayerInputController : MonoBehaviour {
    private InputManager input;

    [SerializeField] private float interactPersistence = 0.2f;

    //public bool Dash { get; private set; }
    //public bool LookBack { get; private set; }
    //public bool Interact { get; private set; }
    public bool Attack { get; private set; }
    public Vector3 Movement { get; private set; }
    public Vector2 LookDirection { get; private set; }

    //private Cooldown dashCooldown;
    private Cooldown attackCooldown;

    // Mouse movement transformation into controller input
    private int width, height;
    private float mouseMoveRadius;
    private Vector2 halfScreenSize;

    private void OnEnable() {
        UpdateScreenSizeValues();

        input = InputManager.Instance;

        input.Player.Move.performed += OnMove;
        input.Player.Look.performed += OnLook;
        input.Player.Attack.performed += OnAttack;
        //input.Player.Dash.performed += OnDash;
        //input.controlSchemeChange += OnControlSchemeChange;
    }

    private void OnDisable() {
        input.Player.Move.performed -= OnMove;
        input.Player.Look.performed -= OnLook;
        input.Player.Attack.performed -= OnAttack;
        //input.Player.Dash.performed -= OnDash;
        //input.Player.OpenPauseMenu.performed -= OpenPauseMenu;
        //input.controlSchemeChange -= OnControlSchemeChange;

        Movement = Vector3.zero;
        //Dash = false;
        Attack = false;
    }

    private void UpdateScreenSizeValues() {
        width = Screen.width;
        height = Screen.height;
        halfScreenSize = new Vector2(width / 2f, height / 2f);
        mouseMoveRadius = Mathf.Min(halfScreenSize.x, halfScreenSize.y) / 2f;
    }

    private void Update() {
        if (width != Screen.width || height != Screen.height) {
            UpdateScreenSizeValues();
        }
    }

    void OnMove(InputAction.CallbackContext ctx) {
        var inp = ctx.ReadValue<Vector2>();
        // For 3D swap y and z
        Movement = new Vector3(inp.x, inp.y, 0);
        if (input.CurrentControlScheme == InputManager.ControlScheme.Keyboard) {
            Movement.Normalize();
        }
    }

    void OnLook(InputAction.CallbackContext ctx) {
        LookDirection = ctx.ReadValue<Vector2>();
        if (input.CurrentControlScheme == InputManager.ControlScheme.Keyboard) {
            // Move the vector to origin being the centre of the screen
            var vec = LookDirection - halfScreenSize;
            var normalisedVec = vec.normalized;
            if (vec.magnitude > mouseMoveRadius) {
                // If ourside of the radius just return the normalised vector
                LookDirection = normalisedVec;
            } else {
                // When inside the radius, remap the vector to be inside the unit circle
                var absoluteNormalisedVec = normalisedVec.Abs();
                var maxValuesInDirectionOfVec = (absoluteNormalisedVec * mouseMoveRadius);
                LookDirection = vec.Remap(-maxValuesInDirectionOfVec, maxValuesInDirectionOfVec, -absoluteNormalisedVec, absoluteNormalisedVec);
            }
        }
    }

    void OnAttack(InputAction.CallbackContext ctx) {
        var isPressed = ctx.ReadValueAsButton();
        if (!isPressed) return;
        Attack = true;
        attackCooldown?.Stop();
        attackCooldown = Cooldown.Wait(interactPersistence).OnComplete(() => {
            Attack = false;
        });
    }

    public void AttackPerformed() {
        Attack = false;
        attackCooldown?.Stop();
    }

    //void OnDash(InputAction.CallbackContext ctx) {
    //    var isPressed = ctx.ReadValueAsButton();
    //    if (!isPressed) return;
    //    Dash = true;
    //    dashCooldown?.Stop();
    //    dashCooldown = Cooldown.Wait(interactPersistence).OnComplete(() => {
    //        Dash = false;
    //    });
    //}

    //public void DashPerformed() {
    //    Dash = false;
    //    dashCooldown.Stop();
    //}
}
