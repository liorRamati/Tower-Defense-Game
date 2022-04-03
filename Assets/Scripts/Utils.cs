using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public Terrain gameTerrain;

    /// <summary>
    /// Checks if a circle with the given radius around the given point is a flat area
    /// </summary>
    /// <returns>true if the circle is flat, false otherwise</returns>
    public bool IsInFlatCircle(Vector3 point, int radius)
    {
        if (radius <= 0)
        {
            return true;
        }

        // get point height relative to terrain
        float pointHeight = point.y / gameTerrain.terrainData.size.y;
        // round x and z coordinates of the point
        int cx = (int)Mathf.Round(point.x), cy = (int)Mathf.Round(point.z);

        // save the scale of the terrain units to heightmap units
        int terrainRes = gameTerrain.terrainData.heightmapResolution;
        float widthSacle = (float)terrainRes / gameTerrain.terrainData.size.x, lengthScale = (float)terrainRes / gameTerrain.terrainData.size.z;
        int rx = (int)Mathf.Round(radius * widthSacle), ry = (int)Mathf.Round(radius * lengthScale);

        // check if part of the circle is beyond the edge of the terrain
        if (cx - radius < 0 || cx + radius >= gameTerrain.terrainData.size.x || cy - radius < 0 || cy + radius >= gameTerrain.terrainData.size.z)
        {
            return false;
        }

        // check if position is above the road
        if (point.y < 5)
        {
            return false;
        }

        // get the heightmap values of the relevent area;
        float[,] heights = gameTerrain.terrainData.GetHeights((int)Mathf.Round((cx - radius) * widthSacle), (int)Mathf.Round((cy - radius) * lengthScale),
            2 * rx + 1, 2 * ry + 1);

        // check the circle heights in the heightmap (the circle is an ellipse in the heightmap coordinates). using midpoint ellipse algorithm.
        int x = 0, y = ry;
        double dx = 0, dy = 2 * rx * rx * y, d1 = (ry * ry) - (rx * rx * ry) + (0.25 * rx * rx);

        // region 1
        while (dx < dy)
        {
            if (d1 < 0)
            {
                x++;
                dx += 2 * ry * ry;
                d1 += dx + (ry * ry);
            }
            else
            {
                // check next line is flat
                for (int i = 0; i < x; i++)
                {
                    if (Mathf.Abs(heights[ry + y, rx + i] - pointHeight) * gameTerrain.terrainData.size.y > 5 ||
                        Mathf.Abs(heights[ry + y, rx - i] - pointHeight) * gameTerrain.terrainData.size.y > 5 ||
                        Mathf.Abs(heights[ry - y, rx + i] - pointHeight) * gameTerrain.terrainData.size.y > 5 ||
                        Mathf.Abs(heights[ry - y, rx - i] - pointHeight) * gameTerrain.terrainData.size.y > 5)
                    {
                        return false;
                    }
                }

                x++;
                y--;
                dx += 2 * ry * ry;
                dy -= 2 * rx * rx;
                d1 += dx - dy + (ry * ry);
            }
        }

        double d2 = (ry * ry * (x + 0.5) * (x + 0.5)) + (rx * rx * (y - 1) * (y - 1)) - (rx * rx * ry * ry);

        while (y >= 0)
        {
            // check next line is flat
            for (int i = 0; i < x; i++)
            {
                if (Mathf.Abs(heights[ry + y, rx + i] - pointHeight) * gameTerrain.terrainData.size.y > 5 ||
                    Mathf.Abs(heights[ry + y, rx - i] - pointHeight) * gameTerrain.terrainData.size.y > 5 ||
                    Mathf.Abs(heights[ry - y, rx + i] - pointHeight) * gameTerrain.terrainData.size.y > 5 ||
                    Mathf.Abs(heights[ry - y, rx - i] - pointHeight) * gameTerrain.terrainData.size.y > 5)
                {
                    return false;
                }
            }

            if (d2 > 0)
            {
                y--;
                dy -= 2 * rx * rx;
                d2 += (rx * rx) - dy;
            }
            else
            {
                y--;
                x++;
                dx += 2 * ry * ry;
                dy -= 2 * rx * rx;
                d2 += dx - dy + (rx * rx);
            }
        }

        return true;
    }

    /// <summary>
    /// maps a value in one range to a value in another range
    /// </summary>
    /// <param name="value">the value to map</param>
    /// <param name="sMin">the minimum of the first range</param>
    /// <param name="sMax">the maximum of the first range</param>
    /// <param name="dMin">the minimum of the second range</param>
    /// <param name="dMax">the maimum of the second range</param>
    /// <returns></returns>
    public float Map(float value, float sMin, float sMax, float dMin = 0f, float dMax = 1f)
    {
        return (value - sMin) * (dMax - dMin) / (sMax - sMin) + dMin;
    }
}
