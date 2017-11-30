using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    /// <summary>
    /// Number of lines going vertically.
    /// </summary>
    public int Width;

    /// <summary>
    /// Number of lines going side to side.
    /// </summary>
    public int Height;

    /// <summary>
    /// current location (X,Y)
    /// </summary>
    public Vector2 Location;

    /// <summary>
    /// Get the next location on the grid.
    /// </summary>
    /// <param name="direction">Direction to go in.</param>
    /// <returns>The next location on the grid you'll be going to.</returns>
    public Vector2 GetNext(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Location + Vector2.up;
            case Direction.Right:
                return Location + Vector2.right;
            case Direction.Down:
                return Location + Vector2.down;
            case Direction.Left:
                return Location + Vector2.left;
        }
        return Location;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
