using System.Collections;
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

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate New Terrain"))
        {
            terrainPlacer.GenerateNewTerrain();
        }

        if (GUILayout.Button("Clear Terrain"))
        {
            terrainPlacer.ClearTerrain();
        }

        if (GUILayout.Button("Save VertexData to Enemies"))
        {
            terrainPlacer.SaveVertexData();
        }

        if (GUILayout.Button("Save Mesh"))
        {
            AssetDatabase.CreateAsset( terrainPlacer.meshFilters[0].mesh, "Assets/Mesh.fbx");
            AssetDatabase.SaveAssets();
        }

        GUILayout.EndHorizontal();

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
