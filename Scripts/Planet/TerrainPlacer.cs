using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class TerrainPlacer : MonoBehaviour
{
    [SerializeField]
    public bool autoUpdate;

    [SerializeField]
    public Vector3Int size = Vector3Int.one * 20;
    public Vector3Int chunkSize = Vector3Int.one * 10;
    [SerializeField]
    public Vector3 offset;

    public float planetRadius;
    public float coreRadius;

    [Range(0, 1)]
    public float placeThreshold = 0.5f;
    public float noiseScale = 0.1f;

    [SerializeField]
    Transform chunkParent;

    public int resolution = 1;
    Vector3Int scaledSize;

    [SerializeField]
    Material material;
    [SerializeField]
    MeshFilter[] meshFilters;

    private void Awake()
    {
        SaveVertexData();
    }

    public void GenerateNewTerrain()
    {
        ClearTerrain();
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        float startTime = Time.realtimeSinceStartup;

        transform.localScale = Vector3.one / resolution;

        offset = new Vector3
        {
            x = Random.Range(0, 1000f),
            y = Random.Range(0, 1000f),
            z = Random.Range(0, 1000f)
        };

        CreateVertexData(out int[,,] vertexData);
        SaveVertexData(vertexData);
        Debug.Log("Vertex Data Created in " + (Time.realtimeSinceStartup - startTime) + " seconds.");

        InitializeMeshData(vertexData);
        Debug.Log("Cubes Created in " + (Time.realtimeSinceStartup - startTime) + " seconds.");

        Debug.Log("Loaded in " + (Time.realtimeSinceStartup - startTime) + " seconds.");
    }

    public void ClearTerrain()
    {
        for (int i = 0; i < meshFilters.Length; i++)
        {
            if (meshFilters[i] != null)
            {
                DestroyImmediate(meshFilters[i].gameObject);
            }
        }

        meshFilters = new MeshFilter[0];

        ClearLog();
    }

    public void SaveVertexData()
    {
        CreateVertexData(out int[,,] vertexData);
        BaseEnemy.planetData = new PlanetData(vertexData, planetRadius, coreRadius);
        Debug.Log("Planet data saved");
    }

    public void SaveVertexData(int[,,] vertexData)
    {
        BaseEnemy.planetData = new PlanetData(vertexData, planetRadius, coreRadius);
        Debug.Log("Planet data saved");
    }

    int[,,] CreateVertexData(out int[,,] vertexData)
    {
        scaledSize = size * resolution;

        Vector3 planetOrigin = new Vector3(scaledSize.x, scaledSize.y, scaledSize.z) / 2;
        vertexData = new int[scaledSize.x + 1, scaledSize.y + 1, scaledSize.z + 1];
        for (int x = 0; x <= scaledSize.x; x++)
        {
            for (int y = 0; y <= scaledSize.y; y++)
            {
                for (int z = 0; z <= scaledSize.z; z++)
                {
                    if (Noise(x, y, z) >= placeThreshold && Mathf.Abs(Vector3.Distance(new Vector3(x, y, z), planetOrigin)) < planetRadius)
                    {
                        vertexData[x, y, z] = 1;
                    }

                    if (Mathf.Abs(Vector3.Distance(new Vector3(x, y, z), planetOrigin)) < coreRadius)
                    {
                        vertexData[x, y, z] = 1;
                    }
                }
            }
        }

        return vertexData;
    }

    MeshFilter[] InitializeMeshData(int[,,] vertices)
    {
        Vector3Int numChunks = new Vector3Int(Mathf.CeilToInt(scaledSize.x / chunkSize.x), Mathf.CeilToInt(scaledSize.y / chunkSize.y), Mathf.CeilToInt(scaledSize.z / chunkSize.z));
        meshFilters = new MeshFilter[numChunks.x * numChunks.y * numChunks.z];

        int meshIndex = 0;
        for (int x = 0; x < numChunks.x; x++)
        {
            for (int y = 0; y < numChunks.y; y++)
            {
                for (int z = 0; z < numChunks.z; z++)
                {
                    LoadChunkMesh(new Vector3Int(x * chunkSize.x, y * chunkSize.y, z * chunkSize.z), vertices, meshIndex);
                    meshIndex++;
                }
            }
        }

        return meshFilters;
    }

    Mesh LoadChunkMesh(Vector3Int chunkOrigin, int[,,] vertexData, int meshIndex)
    {

        GameObject meshObj = new GameObject("Chunk [" + chunkOrigin.x + ", " + chunkOrigin.y + ", " + chunkOrigin.z + "]");
        meshObj.transform.parent = chunkParent;
        meshObj.transform.position = chunkOrigin;

        Vector3[] meshVertices = new Vector3[8 * chunkSize.x * chunkSize.y * chunkSize.z];
        int[] meshTriangles = new int[6 * meshVertices.Length];
        Mesh mesh = new Mesh();

        int vertIndex = 0;
        for (int x = 0; x < chunkSize.x; x++)
        {
            for (int y = 0; y < chunkSize.y; y++)
            {
                for (int z = 0; z < chunkSize.z; z++)
                {
                    int[] newEdgePoints = Table.GetEdgeTableRow(GetTableIndex(new int[] {
                        vertexData[x + chunkOrigin.x, y+chunkOrigin.y, z + chunkOrigin.z],
                        vertexData[x + chunkOrigin.x+1, y+chunkOrigin.y, z + chunkOrigin.z],
                        vertexData[x + chunkOrigin.x+1, y+chunkOrigin.y, z + chunkOrigin.z+1],
                        vertexData[x + chunkOrigin.x, y+chunkOrigin.y, z + chunkOrigin.z+1],
                        vertexData[x + chunkOrigin.x, y+chunkOrigin.y+1, z + chunkOrigin.z],
                        vertexData[x + chunkOrigin.x+1, y+chunkOrigin.y+1, z + chunkOrigin.z],
                        vertexData[x + chunkOrigin.x+1, y+chunkOrigin.y+1, z + chunkOrigin.z+1],
                        vertexData[x + chunkOrigin.x, y+chunkOrigin.y+1, z + chunkOrigin.z+1]
                    }));

                    if (newEdgePoints.Length > 0)
                    {
                        Vector3 cubePos = new Vector3(x, y, z);

                        for (int i = 0; i < newEdgePoints.Length; i++)
                        {
                            AddVertex(ref meshVertices, newEdgePoints[i], cubePos, vertIndex);
                            meshTriangles[vertIndex] = vertIndex;
                            vertIndex++;
                        }
                    }
                }
            }
        }

        System.Array.Resize(ref meshVertices, vertIndex);
        System.Array.Resize(ref meshTriangles, vertIndex);


        if (meshVertices.Length == 0)
        {
            DestroyImmediate(meshObj);
            return null;
        }

        mesh.vertices = meshVertices;
        mesh.triangles = meshTriangles;
        mesh.RecalculateNormals();

        MeshRenderer mr = meshObj.AddComponent<MeshRenderer>();
        mr.sharedMaterial = material;
        meshFilters[meshIndex] = meshObj.AddComponent<MeshFilter>();
        meshFilters[meshIndex].sharedMesh = mesh;
        MeshCollider cldr = meshObj.AddComponent<MeshCollider>();
        cldr.sharedMesh = mesh;

        return mesh;
    }

    Vector3 AddVertex(ref Vector3[] vertices, int edgePoint, Vector3 origin, int vertIndex)
    {
        Vector3 newVertexPos = Vector3.zero;

        switch (edgePoint)
        {
            case 0:
                newVertexPos = new Vector3(origin.x + 0.5f, origin.y, origin.z);
                break;

            case 1:
                newVertexPos = new Vector3(origin.x + 1, origin.y, origin.z + 0.5f);
                break;

            case 2:
                newVertexPos = new Vector3(origin.x + 0.5f, origin.y, origin.z + 1);
                break;

            case 3:
                newVertexPos = new Vector3(origin.x, origin.y, origin.z + 0.5f);
                break;

            case 4:
                newVertexPos = new Vector3(origin.x + 0.5f, origin.y + 1, origin.z);
                break;

            case 5:
                newVertexPos = new Vector3(origin.x + 1, origin.y + 1, origin.z + 0.5f);
                break;

            case 6:
                newVertexPos = new Vector3(origin.x + 0.5f, origin.y + 1, origin.z + 1);
                break;

            case 7:
                newVertexPos = new Vector3(origin.x, origin.y + 1, origin.z + 0.5f);
                break;

            case 8:
                newVertexPos = new Vector3(origin.x, origin.y + 0.5f, origin.z);
                break;

            case 9:
                newVertexPos = new Vector3(origin.x + 1, origin.y + 0.5f, origin.z);
                break;

            case 10:
                newVertexPos = new Vector3(origin.x + 1, origin.y + 0.5f, origin.z + 1);
                break;

            case 11:
                newVertexPos = new Vector3(origin.x, origin.y + 0.5f, origin.z + 1);
                break;
        }

        vertices[vertIndex] = newVertexPos;
        return newVertexPos;
    }

    int GetTableIndex(int[] vertices)
    {
        string binary = "";
        for (int i = 7; i >= 0; i--)
        {
            binary += vertices[i];
        }

        int tableIndex = System.Convert.ToInt32(binary, 2);

        return tableIndex;
    }

    float Noise(float x, float y, float z)
    {
        float noise = PerlinNoise3D.PerlinNoise(new Vector3(x, y, z) * noiseScale, offset);
        return noise;
    }

    public void ClearLog()
    {
        Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        System.Type type = assembly.GetType("UnityEditor.LogEntries");
        MethodInfo method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}

public struct PlanetData
{
    public int[,,] terrainData;
    public float planetRadius;
    public float coreRadius;

    public PlanetData(int[,,] vertexData, float planetRadius, float coreRadius)
    {
        terrainData = vertexData;
        this.planetRadius = planetRadius;
        this.coreRadius = coreRadius;
    }
}