using CooldownManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameMenu : MonoBehaviour {
    public GameObject canv;
    private bool isPaused = false;
    private int total = 4;
    private int current = 0;

    private void Awake() {
        canv.SetActive(false);
    }

    public void PoolCleansed() {
        current += 1;
        if (current >= total) {
            Cooldown.Wait(2).OnComplete(() => {
                Show();
            });
        }
    }

    private void Show() {
        isPaused = !isPaused;
        canv.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void Quit() {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
