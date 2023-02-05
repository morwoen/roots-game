using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public GameObject canv;
    private bool isPaused = false;

    private void Awake() {
        canv.SetActive(false);
    }

    private void OnEnable() {
        InputManager.Instance.Player.OpenPauseMenu.performed += OpenPauseMenu_performed;
    }

    private void OnDisable() {
        InputManager.Instance.Player.OpenPauseMenu.performed -= OpenPauseMenu_performed;
    }

    private void OpenPauseMenu_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        Pause();
    }

    public void Pause() {
        isPaused = !isPaused;
        canv.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void Quit() {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
