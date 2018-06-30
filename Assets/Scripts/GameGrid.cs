using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    public GameObject BoundObject;

    private GameTile[,] _tiles;
    
    private void Start()
    {
        var retrievedTiles = BoundObject.GetComponentsInChildren<GameTile>();

        _tiles = new GameTile[retrievedTiles.Max(tile => tile.Row), retrievedTiles.Max(tile => tile.Col)];
    }
}
