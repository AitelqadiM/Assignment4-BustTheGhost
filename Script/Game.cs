using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

class WeightedRandomBag<T>
{
    private struct Entry
    {
        public double accumulatedWeight;

        public T item;
    }

    private List<Entry> entries = new List<Entry>();

    private double accumulatedWeight;

    private System.Random rand = new System.Random();

    public void AddEntry(T item, double weight)
    {
        accumulatedWeight += weight;
        entries
            .Add(new Entry {
                item = item,
                accumulatedWeight = accumulatedWeight
            });
    }

    public T GetRandom()
    {
        double r = rand.NextDouble() * accumulatedWeight;

        foreach (Entry entry in entries)
        {
            if (entry.accumulatedWeight >= r)
            {
                return entry.item;
            }
        }
        return default(T); //should only happen when there are no entries
    }
}

public class Game : MonoBehaviour
{
    public int height = 6;
    public int width = 6;
    public Tile[,] grid = new Tile[6,6];
    public GameObject[,] Textgrid;
    public double probabilitycount = 2;
    public int gx;
    public int gy;
    public int lastcheckedX;
    public int lastcheckedY;
    public int NumberOfTiles = 0;
    public int NumberOfClicks = 0;

    public String displayed_color(int DistanceFromGhost)
    {
        WeightedRandomBag<String> color_noise = new WeightedRandomBag<String>();

        if (DistanceFromGhost == 3 || DistanceFromGhost == 4)
        {
            color_noise.AddEntry("yellow", 0.5);
            color_noise.AddEntry("orange", 0.2);
            color_noise.AddEntry("red", 0.1);
            color_noise.AddEntry("green", 0.25);

            return color_noise.GetRandom();
        }
        else if (DistanceFromGhost == 1 || DistanceFromGhost == 2)
        {
            color_noise.AddEntry("yellow", 0.2);
            color_noise.AddEntry("orange", 0.5);
            color_noise.AddEntry("red", 0.25);
            color_noise.AddEntry("green", 0.1);

            return color_noise.GetRandom();
        }
        else if (DistanceFromGhost == 0)
        {
            color_noise.AddEntry("yellow", 0.05);
            color_noise.AddEntry("orange", 0.25);
            color_noise.AddEntry("red", 0.6);
            color_noise.AddEntry("green", 0.050);

            return color_noise.GetRandom();
        }
        else if (DistanceFromGhost >= 5)
        {
            color_noise.AddEntry("yellow", 0.25);
            color_noise.AddEntry("orange", 0.05);
            color_noise.AddEntry("red", 0.050);
            color_noise.AddEntry("green", 0.6);

            return color_noise.GetRandom();
        }
        else
            return "green";
    }

    public float JointTableProbability(string color, int DistanceFromGhost)
    {
        //Table 1
        if (
            color.Equals("yellow") &&
            (DistanceFromGhost == 3 || DistanceFromGhost == 4)
        ) return 0.5f;
        if (
            color.Equals("red") &&
            (DistanceFromGhost == 3 || DistanceFromGhost == 4)
        ) return 0.1f;
        if (
            color.Equals("green") &&
            (DistanceFromGhost == 3 || DistanceFromGhost == 4)
        ) return 0.25f;
        if (
            color.Equals("orange") &&
            (DistanceFromGhost == 3 || DistanceFromGhost == 4)
        ) return 0.2f;

        //Table2
        if (
            color.Equals("yellow") &&
            (DistanceFromGhost == 1 || DistanceFromGhost == 2)
        ) return 0.2f;
        if (
            color.Equals("red") &&
            (DistanceFromGhost == 1 || DistanceFromGhost == 2)
        ) return 0.25f;
        if (
            color.Equals("green") &&
            (DistanceFromGhost == 1 || DistanceFromGhost == 2)
        ) return 0.1f;
        if (
            color.Equals("orange") &&
            (DistanceFromGhost == 1 || DistanceFromGhost == 2)
        ) return 0.5f;

        //Table3
        if (color.Equals("yellow") && DistanceFromGhost >= 5) return 0.25f;
        if (color.Equals("red") && DistanceFromGhost >= 5) return 0.05f;
        if (color.Equals("green") && DistanceFromGhost >= 5) return 0.6f;
        if (color.Equals("orange") && DistanceFromGhost >= 5) return 0.05f;

        //Table4
        if (color.Equals("red") && DistanceFromGhost == 0) return 0.6f;
        if (color.Equals("yellow") && DistanceFromGhost == 0) return 0.05f;
        if (color.Equals("green") && DistanceFromGhost == 0) return 0.05f;
        if (color.Equals("orange") && DistanceFromGhost == 0) return 0.25f;

        return 0;
    }

    void Start()
    {
        this.Textgrid = new GameObject[6, 6];
        PlaceGhost();
    }

    public void CheckInputGrid()
    {
        int
            Distance = 0,
            Dis_X = 0,
            Dis_Y = 0;

        if (Input.GetButtonDown("Fire1"))
        {

            Vector3 mousePosition =
                Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int a = Mathf.RoundToInt(mousePosition.x);
            int b = Mathf.RoundToInt(mousePosition.y);
            if (a > 6 || b > 6 || a < 0 || b < 0)
            {
                return;
            }
            NumberOfClicks++;
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int x = Mathf.RoundToInt(mousePosition.x);
            int y = Mathf.RoundToInt(mousePosition.y);
            lastcheckedX = x;
            lastcheckedY = y;

            Dis_X = Math.Abs(lastcheckedX - gx);

            Dis_Y = Math.Abs(lastcheckedY - gy);

            Distance = Dis_X + Dis_Y;

            Tile tile = grid[x, y];

          	string colorSquare= displayed_color(Distance);
          	float Sum =0;
          	float Summ = 0;
          	
          	if (tile.iscovered == true)
            {
                for (int aa = 0; aa < 6; a++){
                    for(int bb = 0; bb < 6; b++){
                    Dis_Y = Math.Abs(aa - gy);
                    Dis_X = Math.Abs(bb - gx);
                    Distance = Dis_X + Dis_Y;  
                    
                    float newproba = float.Parse(grid[bb,aa].probability.text);
                    String newcolor = displayed_color(Distance);
                    if (NumberOfClicks == 1) {
                        newproba = 0.027f;
                    }
                    grid[bb, aa].probability.text =
                        (newproba * JointTableProbability(newcolor,Distance)).ToString();

                    Sum += float.Parse(grid[bb, aa].probability.text);
                    }
                }
                
            }
            Debug.Log("Sum");
            Debug.Log(Sum);
            for (int aa = 0; aa < 6; aa++)
            {
                for (int bb = 0; bb < 6; bb++)
                {
                    {
                        float newproba = float.Parse(grid[b, a].probability.text);
                        grid[bb, aa].probability.text = (newproba / Sum).ToString();

                            Summ += float.Parse(grid[bb, aa].probability.text);
                        }
                    }
                }            
            tile.SetIsCovered(false);
        }
    }

    public void PlaceGhost()
    {
        int x = UnityEngine.Random.Range(0, 6);
        int y = UnityEngine.Random.Range(0, 6);
        if (grid[x, y] == null)
        {
            Tile ghostTile =
                Instantiate(Resources.Load("Prefabs/red", typeof (Tile)),
                new Vector3(x, y, 0),
                Quaternion.identity) as
                Tile;
            grid[x, y] = ghostTile;
            grid[x, y].probability.text = "0.027";

            gx = x;
            gy = y;
            Debug.Log("(" + gx + ", " + gy + ")");
            PlaceColor (x, y);
        }
    }

    public void PlaceColor(int X, int Y)
    {
        int
            Dis_X = 0,
            Dis_Y = 0,
            Distance = 0;
        for (int y = 0; y < 6; y++)
        {
            for (int x = 0; x < 6; x++)
            {
                Dis_X = Math.Abs(x - X);

                Dis_Y = Math.Abs(y - Y);

                Distance = Dis_X + Dis_Y;

                String color = displayed_color(Distance);

                String path = string.Format("Prefabs/{0}", color);

                if (grid[x, y] == null)
                {
                    Tile colorTile =
                        Instantiate(Resources.Load(path, typeof (Tile)),
                        new Vector3(x, y, 0),
                        Quaternion.identity) as
                        Tile;
                    grid[x, y] = colorTile;

                    grid[x, y].probability.text = "0.027";
                    NumberOfTiles++;
                }
            }
        }
    }
}