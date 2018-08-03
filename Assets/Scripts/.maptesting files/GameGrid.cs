using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    public GameObject BoundObject;

    private GameTile[,] _tiles;

    public Transform _hoverEffectObject;
    public GameObject _highlightEffectObj;

    int[] _gridDimensions;

    Stack<Transform> _highlightObjects=new Stack<Transform>();
    Stack<GameTile> _selectedTiles=new Stack<GameTile>();

    private void Start()
    {
        GameTile[] retrievedTiles = BoundObject.GetComponentsInChildren<GameTile>();

        //get the lowest coordinate of the tiles
        int xmin=retrievedTiles.Min(tile => tile.Row);
        int zmin=retrievedTiles.Min(tile => tile.Col);
        Debug.LogFormat("tile min coordinate: {0},{1}",xmin,zmin);

        //determine the dimensions of the grid, calculated by the max minus the min +1
        //manually confirm that this is correct (cross check with physical map)
        int xdim=retrievedTiles.Max(tile => tile.Row)-xmin+1;
        int zdim=retrievedTiles.Max(tile => tile.Col)-zmin+1;

        _gridDimensions=new int[2]{xdim,zdim}; //maybe eventually remove xdim/zdim and just use this
        Debug.LogFormat("tile array size: [{0},{1}]",xdim,zdim);

        _tiles = new GameTile[xdim,zdim];

        //assign the retrieved tiles to the tile array
        int currentCoordx;
        int currentCoordz;
        for (int x=0;x<retrievedTiles.Length;x++)
        {
            //offset the coordinate to 0 (so it is possible for tiles to have negative x/z values)
            currentCoordx=retrievedTiles[x].Row-xmin;
            currentCoordz=retrievedTiles[x].Col-zmin;
            retrievedTiles[x].offsetRowCol(xmin,zmin);

            //detect tiles with identical coordinates (round error occured)
            //if the dimensions calculated earlier are correct and there are no duplicate coordinates,
            //it should guarantee that every tile position is correct
            if (_tiles[currentCoordx,currentCoordz])
            {
                Debug.LogFormat("conflict at {0},{1}",currentCoordx+xmin,currentCoordz+zmin);
                Debug.LogFormat("real coord: {0},{1}",retrievedTiles[x].transform.position.x,retrievedTiles[x].transform.position.z);
                Debug.LogFormat("real coord rounded: {0},{1}",
                    Mathf.Round(retrievedTiles[x].transform.position.x),
                    Mathf.Round(retrievedTiles[x].transform.position.z));
            }

            _tiles[currentCoordx,currentCoordz]=retrievedTiles[x];
        }

        initialiseChars();
    }

    void gridCoverTest(int xdim,int zdim)
    {
        string res="";
        for (int x=0;x<xdim;x++)
        {
            for (int y=0;y<zdim;y++)
            {
                if (_tiles[x,y]!=null)
                {
                    res+="X ";
                }

                else
                {
                    res+="O ";
                }
            }

            res+="\n";
        }

        print(res);
    }

    public void setCharacter(int xpos,int zpos,pchar character)
    {
        _tiles[xpos,zpos].occupyChar=character;
        character.transform.position=new Vector3(_tiles[xpos,zpos].transform.position[0],character.transform.position[1],_tiles[xpos,zpos].transform.position[2]);
    }

    //go through character array and place them
    void initialiseChars()
    {
        pchar[] pchars=GameObject.Find("chars").GetComponentsInChildren<pchar>();

        for (var x=0;x<pchars.Length;x++)
        {
            pchars[x].setLocation();
        }
    }

    //return a game tile at a position, null if out of bounds
    public GameTile getTile(int x,int z)
    {
        if (x<0 || z<0 || x>=_gridDimensions[0] || z>=_gridDimensions[1])
        {
            return null;
        }

        return _tiles[x,z];
    }

    //calls the hover effect over to the specified tile
    public void hoverEffect(int x,int z)
    {
        if (!getTile(x,z))
        {
            return;
        }

        _hoverEffectObject.transform.position=new Vector3(_tiles[x,z].transform.position[0],_tiles[x,z].transform.position[1]+.1f,
            _tiles[x,z].transform.position[2]);
    }

    //given a tile place a highlight effect on it and mark it as selectable
    public void highlightEffect(GameTile tile)
    {
        // Instantiate(_hoverEffectObject,tile.transform.position,Quaternion.Euler(-90,0,0));
        Vector3 p=tile.transform.position;
        _highlightObjects.Push(Instantiate(_hoverEffectObject,new Vector3(p[0],p[1]+.1f,p[2]),Quaternion.Euler(90,0,0)));
        _selectedTiles.Push(tile);
        tile.selectable=true;
    }

    //clear all highlight effects
    //should change later to set the tiles to not selectable?
    public void clearHighlightEffects()
    {
        while (_highlightObjects.Count>0)
        {
            Destroy(_highlightObjects.Pop().gameObject);
            _selectedTiles.Pop().selectable=false;
        }
    }

    //given a start coordinate and some options, perform the callback on tiles in a straight line
    //until out of bounds or the callback says to stop (return false)
    //options:
    //xpos: start X coordinate
    //zpos: start Z coordinate
    //directionXZ: go in X direction or Z direction (0 for X direction, 1 for Z)
    //lineSpacing: spacing of line. 1 is every tile, 2 is every other tile, ect. Can be negative (to go backwards).
    //bool callback(GameTile): performed on every tile. return FALSE to end the query
    public void lineQuery(int xpos,int zpos,int directionXZ,int lineSpacing,System.Func<GameTile,bool> callback)
    {
        if (lineSpacing<=0)
        {
            return;
        }

        int xinc=1;
        int zinc=0;

        if (directionXZ>0)
        {
            xinc=0;
            zinc=1;
        }

        xinc*=lineSpacing;
        zinc*=lineSpacing;

        GameTile currentTile;

        while (true)
        {
            xpos+=xinc;
            zpos+=zinc;

            currentTile=getTile(xpos,zpos);

            if (!currentTile || !callback(currentTile))
            {
                return;
            }
        }
    }

    //select tiles in a diamond range
    //xpos,zpos: starting coordinates
    //range: range of diamond selection
    //box: include diagonals
    //bool callback(GameTile): callback to perform on each tile. return FALSE to end query early
    public void rangeQuery(int xpos,int zpos,int range,int box,System.Func<GameTile,bool> callback)
    {
        if (range<=0)
        {
            return;
        }

        for (int x=(-range);x<=range;x++)
        {
            for (int y=(-range);y<=range;y++)
            {
                if (Mathf.Abs(x)+Mathf.Abs(y)<=range && getTile(xpos+x,zpos+y) && !callback(_tiles[xpos+x,zpos+y]))
                {
                    return;
                }
            }
        }
    }
}