using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour {
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject quitButton;
  public GameObject canv;
  private bool canvBool;
    private void Awake() {
#if UNITY_WEBGL
        Destroy(quitButton);
#endif
    }

    public void QuitGame() {
        audioSource.Play();
        Application.Quit();
    }

    public void PlayGame() {
        audioSource.Play();
        SceneManager.LoadScene(1);
    }

private void OnEnable()
  {
    InputManager.Instance.Player.OpenPauseMenu.performed += OpenPauseMenu_performed;
  }
  private void OnDisable()
  {
    InputManager.Instance.Player.OpenPauseMenu.performed -= OpenPauseMenu_performed;
  }
  private void OpenPauseMenu_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
  {
    canvBool = !canvBool;
    canv.SetActive(canvBool);
  }
}
