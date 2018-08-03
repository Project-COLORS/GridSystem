using System;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    public int Row { get; private set; }
    public int Col { get; private set; }
    public bool IsInitialized { get; private set; }
    public TileData Properties;
    public event EventHandler OnInitialized;

    private Material _material;
    private Vector3 _initialPosition;

    public GameTile()
    {
        IsInitialized = false;
    }

    private void Start()
    {
        Row = (int) transform.position.x;
        Col = (int) transform.position.z;
        _initialPosition = transform.position;

        Properties.OnCurrentColorChange += UpdateTileColor;

        IsInitialized = true;
        Debug.Log("Tile initialized.");
        if (OnInitialized != null)
        {
            OnInitialized(this, EventArgs.Empty);
        }
    }

    private void UpdateTileColor(object source, TileData.ColorChangeEventArgs changeData)
    {
        GetComponent<Renderer>().material = MaterialCache.getTileMaterial(changeData.NewColor);
    }

    // Comment this function out if you want to disable the animation
    private void FixedUpdate()
    {
        if (IsInitialized)
        {
            var changeVector = new Vector3(0, 0.10f * (float) Math.Sin(-Time.fixedTime + Row + Col), 0);
            transform.position = _initialPosition + changeVector;
        }
    }
}
