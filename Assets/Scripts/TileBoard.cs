using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    private TileGrid _grid;
    private List<Tile> _tiles;
    private bool _waiting;

    public Tile TilePrefab;
    public TileState[] TileStates;
    public AlphaTileState[] AlphabetTileStates;
    public GameManager GameManager;
    public PopUpManager PopUpManager;

    private Vector2 _startTouchPosition, _endTouchPosition;
    private float _minSwipeDistance = 50f;
    private bool _hasMoved;
    private bool _hasMerged;
    private float _gameMode;

    private Dictionary<string, int> letterToValue = new Dictionary<string, int>
    {
        { "A", 2 },
        { "B", 4 },
        { "C", 8 },
        { "D", 16 },
        { "E", 32 },
        { "F", 64 },
        { "G", 128 },
        { "H", 256 },
        { "I", 512 },
        { "J", 1024 },
        { "K", 2048 },
    };
    private Dictionary<int, string> valueToLetter = new Dictionary<int, string>
    {
        { 2, "A" },
        { 4, "B" },
        { 8, "C" },
        { 16, "D" },
        { 32, "E" },
        { 64, "F" },
        { 128, "G" },
        { 256, "H" },
        { 512, "I" },
        { 1024, "J" },
        { 2048, "K" },
    };

    private void Awake()
    {
        _grid = GetComponentInChildren<TileGrid>();
        _tiles = new List<Tile>(16);
        _gameMode = PlayerPrefs.GetFloat("GameMode", 0);
    }

    public void CreateTile()
    {
        _gameMode = PlayerPrefs.GetFloat("GameMode", 0);
        Tile tile = Instantiate(TilePrefab, _grid.transform);
        switch (_gameMode)
        {
            case 0:
                tile.SetState(TileStates[0], 2);
                tile.Spawn(_grid.GetRandomEmptyCell());
                _tiles.Add(tile);
                break;
            case 1:
                tile.SetStateAlphabet(AlphabetTileStates[0], "A");
                tile.Spawn(_grid.GetRandomEmptyCell());
                _tiles.Add(tile);
                break;
        }

    }

    public void CreateTileAlphabet()
    {
        Tile alphaTile = Instantiate(TilePrefab, _grid.transform);
        alphaTile.SetStateAlphabet(AlphabetTileStates[0], "A");
        alphaTile.Spawn(_grid.GetRandomEmptyCell());
        _tiles.Add(alphaTile);
    }

    private void Update()
    {
        _gameMode = PlayerPrefs.GetFloat("GameMode", 0);
        if (!_waiting)
        {
            _hasMoved = false;
            _hasMerged = false;

            if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && !PopUpManager.SwipeLocked)
            {
                MoveTiles(Vector2Int.up, 0, 1, 1, 1);
            }
            else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && !PopUpManager.SwipeLocked)
            {
                MoveTiles(Vector2Int.down, 0, 1, _grid.Height - 2, -1);
            }
            else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && !PopUpManager.SwipeLocked)
            {
                MoveTiles(Vector2Int.left, 1, 1, 0, 1);
            }
            else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && !PopUpManager.SwipeLocked)
            {
                MoveTiles(Vector2Int.right, _grid.Width - 2, -1, 0, 1);
            }

            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    _startTouchPosition = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    _endTouchPosition = touch.position;
                    Vector2 swipeDirection = _endTouchPosition - _startTouchPosition;

                    if (swipeDirection.magnitude >= _minSwipeDistance && !PopUpManager.SwipeLocked)
                    {
                        swipeDirection.Normalize();

                        if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
                        {
                            if (swipeDirection.x > 0 && PopUpManager)
                            {
                                MoveTiles(Vector2Int.right, _grid.Width - 2, -1, 0, 1);
                            }
                            else
                            {
                                MoveTiles(Vector2Int.left, 1, 1, 0, 1);
                            }
                        }
                        else
                        {
                            if (swipeDirection.y > 0)
                            {
                                MoveTiles(Vector2Int.up, 0, 1, 1, 1);
                            }
                            else
                            {
                                MoveTiles(Vector2Int.down, 0, 1, _grid.Height - 2, -1);
                            }
                        }
                    }
                }
            }

            if (_hasMerged)
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayMergeSound();
            }
            else if (_hasMoved)
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayMoveSound();
            }
        }
    }

    private void MoveTiles(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool changed = false;

        for (int x = startX; x >= 0 && x < _grid.Width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < _grid.Height; y += incrementY)
            {
                TileCell cell = _grid.GetCell(x, y);

                if (cell.Occupied)
                {
                    changed |= MoveTile(cell.Tile, direction);
                }
            }
        }

        if (changed)
        {
            StartCoroutine(WaitForChanges());
        }
    }

    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = _grid.GetAdjacentCell(tile.Cell, direction);

        while (adjacent != null)
        {
            if (adjacent.Occupied)
            {
                if (_gameMode == 0)
                {
                    if (CanMerge(tile, adjacent.Tile))
                    {
                        Merge(tile, adjacent.Tile);
                        _hasMerged = true;
                        return true;
                    }
                }
                else if (_gameMode == 1)
                {
                    if (CanMergeAlpha(tile, adjacent.Tile))
                    {
                        MergeAlphabet(tile, adjacent.Tile);
                        _hasMerged = true;
                        return true;
                    }
                }
                break;
            }

            newCell = adjacent;
            adjacent = _grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            _hasMoved = true;
            return true;
        }
        return false;
    }

    private bool CanMerge(Tile a, Tile b)
    {
        return (a.Number == b.Number) && !b.Locked;
    }
    private bool CanMergeAlpha(Tile a, Tile b)
    {
        return a.Alphabet == b.Alphabet && !b.Locked;
    }

    private void Merge(Tile a, Tile b)
    {
        _tiles.Remove(a);
        a.Merge(b.Cell);

        int index = Mathf.Clamp(IndexOf(b.State) + 1, 0, TileStates.Length - 1);
        int number = b.Number * 2;
        b.SetState(TileStates[index], number);
        GameManager.IncreaseScore(number);
    }

    private void MergeAlphabet(Tile a, Tile b)
    {
        _tiles.Remove(a);
        a.Merge(b.Cell);
        int alphabetIndex = AlphabetIndexOf(b.AlphaSate);

        int index = Mathf.Clamp(alphabetIndex + 1, 0, AlphabetTileStates.Length - 1);
        int number = letterToValue[b.Alphabet] * 2;

        b.SetStateAlphabet(AlphabetTileStates[index], valueToLetter[number]);
        GameManager.IncreaseScore(number);
    }

    private int IndexOf(TileState state)
    {
        for (int i = 0; i < TileStates.Length; i++)
        {
            if (state == TileStates[i])
            {
                return i;
            }
        }
        return -1;
    }

    private int AlphabetIndexOf(AlphaTileState state)
    {
        for (int i = 0; i < AlphabetTileStates.Length; i++)
        {
            if (state == AlphabetTileStates[i])
            {
                return i;
            }
        }
        return -1;
    }

    private IEnumerator WaitForChanges()
    {
        _waiting = true;

        yield return new WaitForSeconds(0.15f);

        _waiting = false;

        foreach (var tile in _tiles)
        {
            tile.Locked = false;
        }

        if (_tiles.Count != _grid.Size)
        {
            CreateTile();
        }

        if (CheckForGameOver())
        {
            GameManager.GameOver();
        }
    }

    private bool CheckForGameOver()
    {
        if (_tiles.Count != _grid.Size)
        {
            return false;
        }

        foreach (var tile in _tiles)
        {
            TileCell up = _grid.GetAdjacentCell(tile.Cell, Vector2Int.up);
            TileCell down = _grid.GetAdjacentCell(tile.Cell, Vector2Int.down);
            TileCell left = _grid.GetAdjacentCell(tile.Cell, Vector2Int.left);
            TileCell right = _grid.GetAdjacentCell(tile.Cell, Vector2Int.right);

            if ((up != null && CanMerge(tile, up.Tile)) && CanMergeAlpha(tile, up.Tile) ||
                (down != null && CanMerge(tile, down.Tile) && CanMergeAlpha(tile, down.Tile)) ||
                (left != null && CanMerge(tile, left.Tile) && CanMergeAlpha(tile, left.Tile)) ||
                (right != null && CanMerge(tile, right.Tile) && CanMergeAlpha(tile, right.Tile)))
            {
                return false;
            }
        }
        return true;
    }

    public void ClearBoard()
    {
        foreach (var cell in _grid.Cells)
        {
            cell.Tile = null;
        }

        foreach (var tile in _tiles)
        {
            Destroy(tile.gameObject);
        }

        _tiles.Clear();
    }
}
