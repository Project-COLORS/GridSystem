using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameGrid : MonoBehaviour
{
    public GameObject BoundObject;
    public event EventHandler OnInitialized;
    public bool IsInitialized { get; private set; }

    private int _totalTiles;
    private int _initializedTiles;
    private GameTile[] _initialTileList;
    private GameTile[,] _tiles;
    private int _rows;
    private int _cols;

    public GameGrid()
    {
        IsInitialized = false;
    }

    private void Start()
    {
        _initializedTiles = 0;
        _initialTileList = BoundObject.GetComponentsInChildren<GameTile>();
        _totalTiles = _initialTileList.Length;
        foreach (var tile in _initialTileList)
        {
            if (tile.IsInitialized)
            {
                _initializedTiles++;
            }
            else
            {
                tile.OnInitialized += AttemptToInitializeAfterTiles;
            }
        }

        if (_initializedTiles == _totalTiles)
        {
            DoInitialize();
        }
    }

    /// <summary>
    /// Retrieves a tile from the grid, or null if out of bounds
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns>The requested game tile, or null</returns>
    [CanBeNull]
    public GameTile GetTile(int row, int col)
    {
        if (row >= 0 && row < _rows && col >= 0 && col < _cols)
        {
            return _tiles[row, col];
        }

        return null;
    }

    /// <summary>
    /// Returns the dimensions of the grid.
    /// </summary>
    /// <returns>An array of the format [# rows, # columns]</returns>
    public Dimensions GetDimensions()
    {
        return new Dimensions(_rows, _cols);
    }

    public class Dimensions
    {
        public readonly int Rows;
        public readonly int Columns;

        public Dimensions(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
        }
    }

    private void AttemptToInitializeAfterTiles(object source, EventArgs e)
    {
        _initializedTiles++;
        if (_initializedTiles != _totalTiles) return;

        DoInitialize();
    }

    private void DoInitialize()
    {
        //get the lowest coordinate of the tiles
        int xmin=_initialTileList.Min(tile => tile.Row);
        int zmin=_initialTileList.Min(tile => tile.Col);
        Debug.LogFormat("tile min coordinate: {0},{1}",xmin,zmin);

        // Need to add one because the highest index will always be # rows/cols - 1
        //also subtract by the lowest coordiante, to offset it in case the lowest coordinate
        //is not 0,0
        _rows = _initialTileList.Max(tile => tile.Row) - xmin + 1;
        _cols = _initialTileList.Max(tile => tile.Col) - zmin + 1;

        _tiles = new GameTile[_rows, _cols];

        foreach (var tile in _initialTileList)
        {
            tile.offsetRowCol(xmin,zmin);

            //detect tiles with identical coordinates (round error occured)
            //if the dimensions calculated earlier are correct and there are no duplicate coordinates,
            //it should guarantee that every tile position is correct
            if (_tiles[tile.Row,tile.Col])
            {
                Debug.LogFormat("conflict at {0},{1}",tile.Row+xmin,tile.Col+zmin);
                Debug.LogFormat("real coord: {0},{1}",tile.transform.position.x,tile.transform.position.y,tile.transform.position.z);
                Debug.LogFormat("real coord rounded: {0},{1}",
                    Mathf.Round(tile.transform.position.x),
                    Mathf.Round(tile.transform.position.z));
            }

            _tiles[tile.Row, tile.Col] = tile;
        }

        _initialTileList = null;
        IsInitialized = true;
        Debug.Log("Grid initialized.");
        if (OnInitialized != null)
        {
            OnInitialized(this, EventArgs.Empty);
        }
    }
}
