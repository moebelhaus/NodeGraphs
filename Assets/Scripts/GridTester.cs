using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GridTester : MonoBehaviour
{
    public Texture2D gridNodeTexture;
    public enum modeLeftClick
    {
        IncreaseValue,
        AddHeat
    }
    [SerializeField] private modeLeftClick _modeLeftClick;

    private Grid grid;
    // Start is called before the first frame update
    private void Start()
    {
        grid = new Grid(20, 20, 5f, new Vector3 (-50, -50, 0), gridNodeTexture);
    }

    private void Update()
    {
        switch (_modeLeftClick)
        {
            case modeLeftClick.IncreaseValue:
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        
                        int value_curr = grid.GetValue(hit.point);
                        grid.SetValue(hit.point, value_curr + 1);
                        
                    }
                }
                break;

            case modeLeftClick.AddHeat:
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        int range = 5;
                        int value = 25;
                        grid.GetXY(hit.point, out int origin_x, out int origin_y);
                        for (int x=0; x<range; x++)
                        {
                            for (int y=0; y<range-x; y++)
                            {
                                float dist =(float)Mathf.Max(x, y) + 1.0f;
                                float mult = 1f / dist;
                                int value_falloff = (int)((float)value * mult);

                                
                                grid.AddValue(origin_x + x, origin_y + y, value_falloff);
                                if (x != 0)
                                {
                                    grid.AddValue(origin_x - x, origin_y + y, value_falloff);
                                }
                                if (y != 0)
                                {
                                    grid.AddValue(origin_x + x, origin_y - y, value_falloff);
                                }
                                if (x != 0 && y != 0)
                                {
                                    grid.AddValue(origin_x - x, origin_y - y, value_falloff);
                                }
                                
                                //grid.AddValue(origin_x, origin_y, 1);
                                
                            }
                        }
                    }
                }
                break;            

            default:
                break;
        }

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log(grid.GetValue(hit.point));
            }
        }
    }
}
