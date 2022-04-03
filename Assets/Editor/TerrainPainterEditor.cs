using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainPainter))]
public class TerrainPainterEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TerrainPainter myScript = (TerrainPainter)target;
        if (GUILayout.Button("Paint Terrain"))
        {
            myScript.Paint();
        }
    }
}
