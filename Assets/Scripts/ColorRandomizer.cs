using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;

public class ColorRandomizer : MonoBehaviour
{
	private GameGrid _grid;
	private GameGrid.Dimensions _dimensions;
	private int _row;
	private int _col;
	private int _counter;
	private TileData.TileColor[] _tileColors;
	private Random _rng;
	private bool _gridInitialized;
	
	// Use this for initialization
	private void Start ()
	{
		_gridInitialized = false;
		_grid = GameObject.Find("Grid").GetComponent<GameGrid>();

		if (!_grid.IsInitialized)
		{
            _grid.OnInitialized += GetGridPropertiesAndBeginUpdates;
		}
		else
		{
			GetGridPropertiesAndBeginUpdates(this, EventArgs.Empty);
		}
	}

	private void GetGridPropertiesAndBeginUpdates(object source, EventArgs e)
	{
		Debug.Log("Timer initializing.");
		_dimensions = _grid.GetDimensions();
		_row = 0;
		_col = 0;
		_counter = 0;
		_tileColors = (TileData.TileColor[]) Enum.GetValues(typeof(TileData.TileColor));
		_rng = new Random();
		_gridInitialized = true;
	}

	private void FixedUpdate()
	{
		// On every 5th fixed update, change a tile color after the grid is initialized
		if (!_gridInitialized || ++_counter != 5) return;
		
		_counter = 0;
		var tile = _grid.GetTile(_row, _col);

		// GetTile returns null if the requested tile is out-of-bounds
		if (tile != null)
		{
			var newColor = _tileColors[_rng.Next(_tileColors.Length)];
			tile.Properties.CurrentColor = newColor;
		}

		// Switch to next column, wrapping to the next row if necessary
		_col++;
		if (_col < _dimensions.Columns) return;
		
		_col = 0;
		_row = (_row + 1) % _dimensions.Rows;
	}
}
