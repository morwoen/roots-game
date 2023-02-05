using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class InputManager : MonoBehaviour {
    public static InputManager Instance;

    public enum ActionMap {
        Player,
        Menu,
    }

    public enum ControlScheme {
        Keyboard,
        Gamepad,
    }

    public delegate void ControlSchemeChange(ControlScheme newControlScheme);
    public event ControlSchemeChange controlSchemeChange;

    private PlayerInput input;
    public PlayerActionMap Player { get; private set; }
    public MenuActionMap Menu { get; private set; }
    public ControlScheme CurrentControlScheme { get; private set; }

    private void OnEnable() {
        if (Instance) {
            Destroy(gameObject);
            return;
        } else {
            Instance = this;
        }

        input = GetComponent<PlayerInput>();
        Player = new PlayerActionMap(input.actions.FindActionMap("Player"));
        Menu = new MenuActionMap(input.actions.FindActionMap("Menu"));
    }

    public void SwitchTo(InternalActionMap map) {
        if (Player.enabled && map != Player) {
            Player.Disable();
        }
        if (Menu.enabled && map != Menu) {
            Menu.Disable();
        }

        map.Enable();
    }

    void OnControlsChanged(PlayerInput playerInput) {
        CurrentControlScheme = playerInput.currentControlScheme.Contains("Keyboard") ? ControlScheme.Keyboard : ControlScheme.Gamepad;
        controlSchemeChange?.Invoke(CurrentControlScheme);

        if (CurrentControlScheme == ControlScheme.Keyboard && !Cursor.visible) {
            Cursor.visible = true;
        } else if (CurrentControlScheme == ControlScheme.Gamepad && Cursor.visible) {
            Cursor.visible = false;
        }
    }

    public class InternalActionMap {
        protected InputActionMap inputActions;

        public bool enabled { get { return inputActions.enabled; } }

        public InternalActionMap(InputActionMap inputActions) {
            this.inputActions = inputActions;
        }

        public void Enable() {
            inputActions.Enable();
        }

        public void Disable() {
            inputActions.Disable();
        }
    }

    public class PlayerActionMap : InternalActionMap {
        public PlayerActionMap(InputActionMap map) : base(map) { }

        public InputAction Move { get { return inputActions.FindAction("Move"); } }
        public InputAction Look { get { return inputActions.FindAction("Look"); } }
        public InputAction Attack { get { return inputActions.FindAction("Attack"); } }
        //public InputAction Dash { get { return inputActions.FindAction("Dash"); } }
        //public InputAction Interact { get { return inputActions.FindAction("Interact"); } }
        public InputAction OpenPauseMenu { get { return inputActions.FindAction("Open Pause Menu"); } }
    }

    public class MenuActionMap : InternalActionMap {
        public MenuActionMap(InputActionMap map) : base(map) { }

        //public InputAction ClosePauseMenu { get { return inputActions.FindAction("ClosePauseMenu"); } }
    }
}
