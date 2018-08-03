//from maptesting!!!!
using UnityEngine;

public class GameTile2 : MonoBehaviour
{
    public int Row { get; private set; }
    public int Col { get; private set; }
    public TileData Properties;

    private Material _material;

    public pchar occupyChar;

    [System.NonSerialized]
    public bool selectable=false;

    private void Start()
    {
        Row = (int) (Mathf.Round(transform.position.x));
        Col = (int) (Mathf.Round(transform.position.z));

        // Debug.LogFormat("{0}, {1}",Row,Col);

        Properties.OnCurrentColorChange += onTileColorChange;
    }

    private void onTileColorChange(object source, TileData.ColorChangeEventArgs changeData)
    {
        GetComponent<Renderer>().material = MaterialCache.getTileMaterial(changeData.NewColor);
    }

    public void tileTest()
    {
        Debug.LogFormat("hey from {0},{1}",Row,Col);
    }

    //shift row/col value to be non negative.
    public void offsetRowCol(int offsetx,int offsetz)
    {
        Row-=offsetx;
        Col-=offsetz;
    }
}
