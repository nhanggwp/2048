using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    public TileState State { get; private set; }
    public AlphaTileState AlphaSate { get; private set; }
    public TileCell Cell { get; private set; }
    public int Number { get; private set; }
    public string Alphabet { get; private set; }
    public bool Locked { get; set; }

    private Image _background;
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _background = GetComponent<Image>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetState(TileState state, int number)
    {
        State = state;
        Number = number;

        _background.color = state.BackgroundColor;
        _text.color = state.TextColor;
        _text.text = number.ToString();
    }

    public void SetStateAlphabet(AlphaTileState state, string alphabet)
    {
        AlphaSate = state;
        Alphabet = alphabet;

        _background.color = state.BackgroundColor;
        _text.color = state.TextColor;
        _text.text = alphabet;
    }

    public void Spawn(TileCell cell)
    {
        if (Cell != null)
        {
            Cell.Tile = null;
        }

        Cell = cell;
        Cell.Tile = this;

        transform.position = cell.transform.position;
        transform.localScale = Vector3.zero;

        DOTween.Kill(transform);
        transform.DOScale(Vector3.one, 0.2f);
    }

    public void MoveTo(TileCell cell)
    {
        if (Cell != null)
        {
            Cell.Tile = null;
        }

        Cell = cell;
        Cell.Tile = this;

        transform.DOMove(cell.transform.position, 0.2f).SetEase(Ease.InOutQuad);
    }

    public void Merge(TileCell cell)
    {
        if (Cell != null)
        {
            Cell.Tile = null;
        }

        Cell = null;
        cell.Tile.Locked = true;

        if (this != null && transform != null)
        {
            DOTween.Kill(transform);
            transform.DOMove(cell.transform.position, 0.15f).SetEase(Ease.Linear).OnComplete(() =>
            {
                if (this != null && transform != null)
                {
                    DOTween.Kill(transform);
                    transform.DOPunchScale(Vector3.one * 0.2f, 0.1f, 10, 1).OnComplete(() =>
                    {
                        if (gameObject != null)
                        {
                            Destroy(gameObject);
                        }
                    });
                }
            });
        }
    }
}
