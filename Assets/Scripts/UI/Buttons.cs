using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
  private string sceneToLoad;
  public GameObject canv;
  private bool canvBool = false;
   public void QuitGame()
  {
    Application.Quit();
    Debug.Log("Quit Game");
  }

  public void PlayGame()
  {
    sceneToLoad = "Gameplay";
    SceneManager.LoadScene(sceneToLoad);
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
