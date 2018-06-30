using UnityEngine;

public class GameTile : MonoBehaviour
{
    public int Row { get; private set; }
    public int Col { get; private set; }
    public TileData Properties;

    private Material _material;
    
    private void Start()
    {
        Row = (int) transform.position.x;
        Col = (int) transform.position.y;

        Properties.OnCurrentColorChange += onTileColorChange;
    }

    private void onTileColorChange(object source, TileData.ColorChangeEventArgs changeData)
    {
        GetComponent<Renderer>().material = MaterialCache.getTileMaterial(changeData.NewColor);
    }
}
