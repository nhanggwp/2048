using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TileBoard Board;
    public CanvasGroup GameOver1;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI HighScoreText;
    public GameObject SettingPanelInstance;

    private int _score;

    private void Awake()
    {
        SettingPanelInstance.SetActive(false);
    }
    private void Start()
    {
        // NewGame();
        GameOver1.alpha = 0f;
        GameOver1.interactable = false;
    }

    public void NewGame()
    {
        SetScore(0);
        HighScoreText.text = LoadHighScore().ToString();
        GameOver1.alpha = 0f;
        GameOver1.interactable = false;
        Board.ClearBoard();
        Board.CreateTile();
        Board.CreateTile();

        Board.enabled = true;
    }

    public void GameOver()
    {
        Board.enabled = false;
        GameOver1.interactable = true;
        StartCoroutine(Fade(GameOver1, 1f, 1f));
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
        AudioManager.Instance.PlayGameOverSound();

    }

    private void SetScore(int score)
    {
        _score = score;
        ScoreText.text = score.ToString();
        SaveHighScore();
    }

    private void SaveHighScore()
    {
        int highscore = LoadHighScore();
        if (_score > highscore)
        {
            PlayerPrefs.SetInt("HighScore", _score);
        }
    }

    private int LoadHighScore()
    {
        return PlayerPrefs.GetInt("HighScore", 0);
    }

    public void IncreaseScore(int points)
    {
        SetScore(_score + points);
    }
}