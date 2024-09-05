using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Unity.Loading;
using Unity.VisualScripting;

public class SceneLoader : MonoBehaviour
{
    public CanvasGroup SceneLoading;
    public Slider Slider;
    public Image FillArea;
    public float LoadingDuration = 3f;
    public Canvas SceneLoad;

    private void Start()
    {
        SceneLoading.interactable = false;
        SceneLoading.alpha = 0f;
        SceneLoad.gameObject.SetActive(false);
    }

    public void StartLoading(string sceneName)
    {
        SceneLoading.interactable = true;
        SceneLoad.gameObject.SetActive(true);

        StartCoroutine(FadeAndAppear(sceneName));
    }
    IEnumerator FadeAndAppear(string sceneName)
    {
        float fadeDuration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            SceneLoading.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            yield return null;
        }

        SceneLoading.alpha = 1;
        yield return (StartCoroutine(LoadingSlider(sceneName)));
    }

    IEnumerator LoadingSlider(string sceneName)
    {
        float elapsedTime = 0;

        while (elapsedTime < LoadingDuration)
        {
            elapsedTime += Time.deltaTime;
            Slider.value = Mathf.Clamp01((elapsedTime / LoadingDuration));

            FillArea.color = Color.Lerp(Color.red, Color.green, Slider.value);
            yield return null;
        }
        SceneManager.LoadScene(sceneName);

    }
}
