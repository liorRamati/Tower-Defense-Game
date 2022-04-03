using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPainter : MonoBehaviour
{
    // values for a height level in the terrain
    [System.Serializable]
    public class SplatHeights
    {
        // the start of the height
        public int startingHeight;
        // the length of the area below the color where the colors gradually change
        public int overlap;
    }

    public SplatHeights[] splatHeights;

    /// <summary>
    /// normalize the element of the given array around its average
    /// </summary>
    void Normalize(float[] arr)
    {
        float total = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            total += arr[i];
        }

        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] /= total;
        }
    }

    /// <summary>
    /// paint the terrain so that different heights will have different color (with some noise)
    /// </summary>
    public void Paint()
    {
        // get the terrain object
        TerrainData terrainData = Terrain.activeTerrain.terrainData;
        // make sure the number of height level match the number of available textures
        if (splatHeights.Length != terrainData.alphamapLayers)
        {
            Debug.LogError("Height array length and number of texture layers does not much");
            return;
        }
        // initiate a map for the colors
        float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        // calculate the color for every point on the terrain
        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                // get point height
                float terrainHeight = terrainData.GetHeight(y, x);

                // calculate how much from each color available to add to the point's color
                float[] splat = new float[splatHeights.Length];

                for (int i = 0; i < splatHeights.Length; i++)
                {
                    // add noise to paint pattern, so it looks more realistic (so there are no staright line where the color changes)
                    float thisNoise = GameManager.gameManager.Utils.Map(Mathf.PerlinNoise(x * 0.01f, y * 0.01f), 0, 1, 0.5f, 1);
                    float thisHeightStart = splatHeights[i].startingHeight * thisNoise - splatHeights[i].overlap * thisNoise;
                    float nextHeightStart = 0;

                    // if not the final color, find the height where the next color should start
                    if (i < splatHeights.Length - 1)
                    {
                        nextHeightStart = splatHeights[i + 1].startingHeight * thisNoise + splatHeights[i + 1].overlap * thisNoise;
                    }

                    // if the point's height is within the values of this color, add some of this color to the point color
                    if ((i == splatHeights.Length - 1 || terrainHeight <= nextHeightStart) && terrainHeight >= thisHeightStart)
                    {
                        // if the point is below the start of this color (in the overlap area), add to this point color some of the current color, according to the
                        // relative height in the overlap area of this color
                        if ((splatHeights[i].startingHeight * thisNoise) - terrainHeight > 0)
                        {
                            splat[i] = 1 - (((splatHeights[i].startingHeight * thisNoise) - terrainHeight) / (splatHeights[i].overlap * thisNoise));
                        }
                        // if the point is above the start of the next color (in the overlap area), to this point color some of the current color, according to the
                        // relative height in the overlap area of the next color
                        else if (i < splatHeights.Length - 1 && terrainHeight - (splatHeights[i + 1].startingHeight * thisNoise) > 0)
                        {
                            splat[i] = 1 - ((terrainHeight - (splatHeights[i + 1].startingHeight * thisNoise)) / (splatHeights[i+1].overlap * thisNoise));
                        }
                        // if the point is in the area where the current color is applied (and not in the overlap area) make the point's color this color fully
                        else
                        {
                            splat[i] = 1;
                        }
                    }
                }

                // normalize the colors, so the sum of the array value is 1
                Normalize(splat);
                
                // fill the color array according to the calculations
                for (int j = 0; j < splatHeights.Length; j++)
                {
                    splatmapData[x, y, j] = splat[j];
                }
            }
        }

        // apply the colors to the terrain
        terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    //private void OnMouseDown()
    //{
    //    int x = (int)Input.mousePosition.x;
    //    int y = (int)Input.mousePosition.y;

    //    Ray r = Camera.main.ScreenPointToRay(new Vector3(x, y, 0));

    //    RaycastHit h;

    //    if (Physics.Raycast(r, out h, 1000.0f))
    //    {

    //        //print(h.point);
            
    //    }
    //}
}
