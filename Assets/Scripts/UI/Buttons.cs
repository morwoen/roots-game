using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour {
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject quitButton;
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
}
