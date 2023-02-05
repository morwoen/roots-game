using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
  private string sceneToLoad;
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
}
