using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class TerrainPlacer : MonoBehaviour
{
    public static TerrainPlacer instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Multiple instances of " + this + " found");
        }

        instance = this;
    }

    public bool autoUpdate;
    public int maxStepCount = 100;

    public Transform chunkParent;

    [SerializeField]
    public Vector3Int size = Vector3Int.one * 20;
    public Vector3Int chunkSize = Vector3Int.one * 10;
    [SerializeField]
    public Vector3 offset;
    Vector3 planetOrigin;

    public float planetRadius;
    public float coreRadius;

    [Range(0, 1)]
    public float placeThreshold = 0.5f;
    public float noiseScale = 0.1f;

    public Material material;
    [SerializeField]
    public MeshFilter[] meshFilters;

    bool meshCreated;

    public Coroutine GenerateNewTerrain()
    {
        StopAllCoroutines();
        ClearTerrain();
        return StartCoroutine(GenerateTerrain());
    }

    IEnumerator GenerateTerrain()
    {
        float startTime = Time.realtimeSinceStartup;

        /*offset = new Vector3
        {
            x = Random.Range(0, 1000f),
            y = Random.Range(0, 1000f),
            z = Random.Range(0, 1000f)
        };*/

        StartCoroutine(InitializeMeshData());
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => meshCreated);
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

        for (int i = chunkParent.childCount - 1; i >= 0; i--)
        {
            Destroy(chunkParent.GetChild(i).gameObject);
        }

        //ClearLog();
    }

    IEnumerator InitializeMeshData()
    {
        planetOrigin = new Vector3(size.x / 2, size.y / 2, size.z / 2);
        Vector3Int numChunks = new Vector3Int(Mathf.CeilToInt(size.x / chunkSize.x), Mathf.CeilToInt(size.y / chunkSize.y), Mathf.CeilToInt(size.z / chunkSize.z));

        meshCreated = false;
        meshFilters = new MeshFilter[numChunks.x * numChunks.y * numChunks.z];

        int meshIndex = -1;
        for (int x = 0; x < numChunks.x; x++)
        {
            for (int y = 0; y < numChunks.y; y++)
            {
                for (int z = 0; z < numChunks.z; z++)
                {
                    meshIndex++;
                    if (meshIndex == meshFilters.Length - 1)
                    {
                        yield return StartCoroutine(LoadChunkMesh(new Vector3Int(x * chunkSize.x, y * chunkSize.y, z * chunkSize.z), meshIndex));
                    }
                    else
                    {
                        StartCoroutine(LoadChunkMesh(new Vector3Int(x * chunkSize.x, y * chunkSize.y, z * chunkSize.z), meshIndex));
                    }
                }
            }
        }


        meshCreated = true;
    }

    IEnumerator LoadChunkMesh(Vector3Int chunkOrigin, int meshIndex)
    {
        GameObject meshObj = new GameObject("Chunk [" + chunkOrigin.x + ", " + chunkOrigin.y + ", " + chunkOrigin.z + "]");
        meshObj.transform.parent = chunkParent;
        meshObj.transform.position = chunkOrigin - new Vector3(size.x / 2, size.z / 2, size.z / 2);

        Vector3[] meshVertices = new Vector3[8 * chunkSize.x * chunkSize.y * chunkSize.z];
        int[] meshTriangles = new int[6 * meshVertices.Length];
        Mesh mesh = new Mesh();

        int vertIndex = 0;
        int step = 0;
        for (int x = 0; x < chunkSize.x; x++)
        {
            for (int y = 0; y < chunkSize.y; y++)
            {
                for (int z = 0; z < chunkSize.z; z++)
                {
                    step++;

                    if (step >= maxStepCount)
                    {
                        step = 0;
                        yield return new WaitForEndOfFrame();
                    }

                    int[] newEdgePoints = Table.GetEdgeTableRow(GetTableIndex(new float[] {
                        Noise(x + chunkOrigin.x, y + chunkOrigin.y, z + chunkOrigin.z),
                        Noise(x + chunkOrigin.x + 1, y + chunkOrigin.y, z + chunkOrigin.z),
                        Noise(x + chunkOrigin.x + 1, y + chunkOrigin.y, z + chunkOrigin.z+1),
                        Noise(x + chunkOrigin.x, y + chunkOrigin.y, z + chunkOrigin.z+1),
                        Noise(x + chunkOrigin.x, y + chunkOrigin.y + 1, z + chunkOrigin.z),
                        Noise(x + chunkOrigin.x + 1, y + chunkOrigin.y + 1, z + chunkOrigin.z),
                        Noise(x + chunkOrigin.x + 1, y + chunkOrigin.y + 1, z + chunkOrigin.z+1),
                        Noise(x + chunkOrigin.x, y + chunkOrigin.y + 1, z + chunkOrigin.z+1)
                    }));

                    if (newEdgePoints.Length > 0)
                    {
                        Vector3 cubePos = new Vector3(x, y, z);

                        for (int i = 0; i < newEdgePoints.Length; i++)
                        {
                            AddEdgeVertex(ref meshVertices, newEdgePoints[i], cubePos + chunkOrigin, chunkOrigin, vertIndex);
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
        }
        else
        {
            mesh.vertices = meshVertices;
            mesh.triangles = meshTriangles;
            mesh.RecalculateNormals();

            MeshRenderer mr = meshObj.AddComponent<MeshRenderer>();
            mr.sharedMaterial = material;
            meshFilters[meshIndex] = meshObj.AddComponent<MeshFilter>();
            meshFilters[meshIndex].sharedMesh = mesh;
            MeshCollider cldr = meshObj.AddComponent<MeshCollider>();
            cldr.sharedMesh = mesh;
        }
    }

    Vector3 AddEdgeVertex(ref Vector3[] vertices, int edgePoint, Vector3 origin, Vector3 chunkOrigin, int vertIndex)
    {
        Vector3Int p1 = new Vector3Int((int)origin.x, (int)origin.y, (int)origin.z);
        Vector3Int p2 = new Vector3Int((int)origin.x, (int)origin.y, (int)origin.z);

        switch (edgePoint)
        {
            case 0:
                p2.x += 1;
                break;

            case 1:
                p1.x += 1;
                p2 += new Vector3Int(1, 0, 1);
                break;

            case 2:
                p1 += new Vector3Int(1, 0, 1);
                p2.z += 1;
                break;

            case 3:
                p2.z += 1;
                break;

            case 4:
                p1.y += 1;
                p2 += new Vector3Int(1, 1, 0);
                break;

            case 5:
                p1 += new Vector3Int(1, 1, 0);
                p2 += new Vector3Int(1, 1, 1);
                break;

            case 6:
                p1 += new Vector3Int(1, 1, 1);
                p2 += new Vector3Int(0, 1, 1);
                break;

            case 7:
                p1.y += 1;
                p2 += new Vector3Int(0, 1, 1);
                break;

            case 8:
                p2.y += 1;
                break;

            case 9:
                p1.x += 1;
                p2 += new Vector3Int(1, 1, 0);
                break;

            case 10:
                p1 += new Vector3Int(1, 0, 1);
                p2 += new Vector3Int(1, 1, 1);
                break;

            case 11:
                p1.z += 1;
                p2 += new Vector3Int(0, 1, 1);
                break;
        }

        Vector3 newEdgeVertexPos = LinearInterpolate(p1, p2) - chunkOrigin;

        vertices[vertIndex] = newEdgeVertexPos;
        return newEdgeVertexPos;
    }

    int GetTableIndex(float[] vertices)
    {
        string binary = "";
        for (int i = 7; i >= 0; i--)
        {
            if (vertices[i] >= placeThreshold)
            {
                binary += 1;
            }
            else
            {
                binary += 0;
            }
        }

        int tableIndex = System.Convert.ToInt32(binary, 2);

        return tableIndex;
    }

    float Noise(float x, float y, float z)
    {
        float distanceFromCenter = Vector3.Distance(new Vector3(x, y, z), planetOrigin);
        float noise = PerlinNoise3D.PerlinNoise(new Vector3(x, y, z) * noiseScale, offset);
        float influence = 1;

        influence += Sigmoid(distanceFromCenter - coreRadius, placeThreshold);
        influence -= Sigmoid(distanceFromCenter - planetRadius, -placeThreshold);

        return Mathf.Clamp01(noise * influence);
    }

    float Noise(Vector3 point)
    {
        float distanceFromCenter = Vector3.Distance(point, planetOrigin);
        float noise = PerlinNoise3D.PerlinNoise(point * noiseScale, offset);
        float influence = 1;

        influence += Sigmoid(distanceFromCenter - coreRadius, placeThreshold);
        influence -= Sigmoid(distanceFromCenter - planetRadius, -placeThreshold);

        return Mathf.Clamp01(noise * influence);
    }

    public bool IsEmptyPoint(Vector3 point)
    {
        return Noise(point) > placeThreshold;
    }

    float Sigmoid(float x, float a = 1)
    {
        return 1 / (1 + Mathf.Exp(a * x));
    }

    Vector3 LinearInterpolate(Vector3Int p1, Vector3Int p2)
    {
        float v1 = Noise(p1.x, p1.y, p1.z);
        float v2 = Noise(p2.x, p2.y, p2.z);

        return p1 + ((placeThreshold - v1) * ((Vector3)p2 - p1) / (v2 - v1));
    }

    /*public void ClearLog()
    {
        Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        System.Type type = assembly.GetType("UnityEditor.LogEntries");
        MethodInfo method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }*/
}