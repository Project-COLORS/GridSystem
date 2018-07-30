using System;
using UnityEngine;

[System.Serializable]
public class TileData
{
    // For use in the unity editor
    public TileColor InitialColor;

    // For use by the program
    private TileColor _backingColor;
    public TileColor CurrentColor
    {
        get { return _backingColor; }
        set
        {
            TileColor oldColor = value;
            _backingColor = value;

            if (OnCurrentColorChange != null)
            {
                OnCurrentColorChange(this, new ColorChangeEventArgs(oldColor, value));
            }
        }
    }
    
    public event EventHandler<ColorChangeEventArgs> OnCurrentColorChange;

    public enum TileColor
    {
        Uncolored = 0, Red = 1, Blue = 2, Yellow = 3, Orange = 4, Purple = 5, Green = 6, Brown = 7
    }

    public TileData()
    {
        CurrentColor = InitialColor;
    }

    public class ColorChangeEventArgs : EventArgs
    {
        public readonly TileColor OldColor;
        public readonly TileColor NewColor;

        public ColorChangeEventArgs(TileColor oldColor, TileColor newColor)
        {
            OldColor = oldColor;
            NewColor = newColor;
        }
    }
}
