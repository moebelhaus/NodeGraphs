using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using Unity.VisualScripting;
using System;

public class Grid
{
    public const int HEAT_MAP_MAX_VALUE = 100;
    public const int HEAT_MAP_MIN_VALUE = 0;
    public const bool showDebug = true; 
    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private int[,] gridArray; //multi dim array
    private TextMesh[,] textMeshArray;
    private GameObject[,] collPlaneArray;
    private Texture2D gridNodeTexture;

    public Grid(int width, int height, float cellSize, Vector3 originPosition, Texture2D gridnodeTexture)
    {
        //member vars set by constructor, use ".this" (?)
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        this.gridNodeTexture = gridnodeTexture;

        gridArray = new int[width, height]; //init array with "width x height" entries
        textMeshArray = new TextMesh[width, height];
        collPlaneArray = new GameObject[width, height];

        Vector3 csh = new Vector3(cellSize, cellSize, 0) * .5f;

        if (gridNodeTexture)
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    textMeshArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y].ToString(), null, GetWorldPosition(x, y) + csh, 20, Color.white, TextAnchor.MiddleCenter);
                    collPlaneArray[x, y] = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    collPlaneArray[x, y].transform.localScale = new Vector3(cellSize/10f, cellSize/10f, cellSize/10f);
                    collPlaneArray[x, y].transform.SetLocalPositionAndRotation(GetWorldPosition(x, y) + csh + new Vector3(0, 0, -.1f), Quaternion.Euler(-90, 0, 0));
                    collPlaneArray[x, y].GetComponent<Renderer>().material.color = gridNodeTexture.GetPixel(0, 0);
                    //collPlaneArray[x, y].GetComponent<Renderer>().material.EnableKeyword("_MainTex");
                    //collPlaneArray[x, y].GetComponent<Renderer>().material.SetTexture("_MainTex", gridNodeTexture);

                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

            OnGridValueChanged += (object sender, OnGridValueChangedEventArgs eventArgs) =>
            {
                textMeshArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y].ToString();
                float uf = (int)(float)gridArray[eventArgs.x, eventArgs.y] / (float)HEAT_MAP_MAX_VALUE * 100f;
                int u = (int)uf;
                collPlaneArray[eventArgs.x, eventArgs.y].GetComponent<Renderer>().material.color = gridNodeTexture.GetPixel(u, 0);
            };
        }
    }

    public int GetWidtht() { return width; }
    public int GetHeight() { return height; }
    public float GetCellSize() { return cellSize; }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        //one way to return multiple values from function: use "out" arguments
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void SetValue(int x, int y, int value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = Mathf.Clamp(value, HEAT_MAP_MIN_VALUE, HEAT_MAP_MAX_VALUE);
            if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
            //textMeshArray[x, y].text = gridArray[x, y].ToString();
        }
    }

    public void SetValue(Vector3 worldPosition, int value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    public void AddValue(int x, int y, int value)
    {
        SetValue(x, y, GetValue(x, y) + value);
    }
    public int GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return default(int);
        }
    }

    public int GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetValue(x, y);
    }
}
