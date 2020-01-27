/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainPlacer))]
public class TerrainPlacerEditor : Editor
{
    TerrainPlacer terrainPlacer;

    public override void OnInspectorGUI()
    {
        GUILayout.Space(10);

        if (true)//(Application.isPlaying)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate New Terrain"))
            {
                terrainPlacer.GenerateNewTerrain();
            }

            if (GUILayout.Button("Clear Terrain"))
            {
                terrainPlacer.ClearTerrain();
            }

            GUILayout.EndHorizontal();
        }

        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();

            if (terrainPlacer.autoUpdate)
            {
                if (check.changed)
                {
                    terrainPlacer.GenerateNewTerrain();
                }
            }
        }
    }

    private void OnEnable()
    {
        terrainPlacer = (TerrainPlacer)target;
    }
}
*/