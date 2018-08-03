using UnityEngine;

public class MaterialCache : MonoBehaviour
{
    private static Material[] _tileMatCache = {null, null, null, null, null, null, null, null};

    public static Material getTileMaterial(TileData.TileColor materialNumber)
    {
        int materialToRetrieve = (int) materialNumber;
        
        if (_tileMatCache[materialToRetrieve] == null)
        {
            Material savedMat;
            switch (materialNumber)
            {
                case TileData.TileColor.Uncolored:
                    savedMat = Resources.Load<Material>("Materials/UncoloredTileMaterial");
                    break;
                case TileData.TileColor.Blue:
                    savedMat = Resources.Load<Material>("Materials/BlueTileMaterial");
                    break;
                case TileData.TileColor.Brown:
                    savedMat = Resources.Load<Material>("Materials/BrownTileMaterial");
                    break;
                case TileData.TileColor.Green:
                    savedMat = Resources.Load<Material>("Materials/GreenTileMaterial");
                    break;
                case TileData.TileColor.Orange:
                    savedMat = Resources.Load<Material>("Materials/OrangeTileMaterial");
                    break;
                case TileData.TileColor.Purple:
                    savedMat = Resources.Load<Material>("Materials/PurpleTileMaterial");
                    break;
                case TileData.TileColor.Red:
                    savedMat = Resources.Load<Material>("Materials/RedTileMaterial");
                    break;
                case TileData.TileColor.Yellow:
                    savedMat = Resources.Load<Material>("Materials/YellowTileMaterial");
                    break;
                default:
                    savedMat = Resources.Load<Material>("Materials/UncoloredTileMaterial");
                    break;
            }

            _tileMatCache[materialToRetrieve] = savedMat;
        }

        return _tileMatCache[materialToRetrieve];
    }
}
