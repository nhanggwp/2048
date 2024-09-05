using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public CanvasGroup OverlayBackGround;

    private void Awake()
    {
        if (OverlayBackGround != null)
        {
            OverlayBackGround.interactable = false;
            OverlayBackGround.alpha = 0f;
        }
    }
    public void LoadNewScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
