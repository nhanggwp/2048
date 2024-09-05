using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.RemoteConfig;
using Firebase.Extensions;

public class ThemeManager : MonoBehaviour
{
    private float _gameMode;

    public GameManager GameManager;

    private void Awake()
    {
        StartCoroutine(InitializeFirebaseAsync());
    }

    private IEnumerator InitializeFirebaseAsync()
    {
        InitialFirebase();
        yield return null;
    }

    private void InitialFirebase()
    {
        var defaults = new Dictionary<string, object>
        {
            { "Game_Theme", "number" }
        };

        FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults).ContinueWithOnMainThread(task =>
        {
            FetchRemoteConfig();
        });
    }

    private void FetchRemoteConfig()
    {
        FirebaseRemoteConfig.DefaultInstance.FetchAsync(System.TimeSpan.Zero).ContinueWithOnMainThread(fetchTask =>
        {
            if (fetchTask.IsCompleted)
            {
                Debug.Log("Remote Config fetched successfully.");
                ActivateRemoteConfig();
            }
            else
            {
                Debug.LogError("Failed to fetch remote config.");
            }
        });
    }

    private void ActivateRemoteConfig()
    {
        FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread(activateTask =>
        {
            if (activateTask.IsCompleted)
            {
                Debug.Log("Remote Config activated.");
                ApplyRemoteConfig();
            }
        });
    }

    public void ApplyRemoteConfig()
    {
        string theme = FirebaseRemoteConfig.DefaultInstance.GetValue("Game_Theme").StringValue;
        if (theme == "number")
        {
            NumberActive();
        }
        else if (theme == "alphabet")
        {
            AlphabetActive();
        }
        else
        {
            Debug.LogWarning("The value fetched from RemoteConfig does not match any play mode.");
        }
    }

    public void AlphabetActive()
    {
        _gameMode = 1;
        PlayerPrefs.SetFloat("GameMode", _gameMode);
        PlayerPrefs.Save();
        Debug.Log($"Mode set to Alphabet. GameMode: {_gameMode}");
        GameManager.NewGame();
    }

    public void NumberActive()
    {
        _gameMode = 0;
        PlayerPrefs.SetFloat("GameMode", _gameMode);
        PlayerPrefs.Save();
        Debug.Log($"Mode set to Number. GameMode: {_gameMode}");
        GameManager.NewGame();
    }
}
