using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    LoadingScreen loadingScreen;
    GravitySource gravitySource;
    TerrainPlacer terrainPlacer;
    Player player;

    private void Start()
    {
        Cursor.visible = false;
        loadingScreen = LoadingScreen.instance;
        gravitySource = GravitySource.instance;
        terrainPlacer = TerrainPlacer.instance;
        player = Player.instance;

        StartCoroutine(GenerateNewPlanet());
    }

    IEnumerator GenerateNewPlanet()
    {
        yield return new WaitForEndOfFrame();
        SpawnEnemy.instance.StopSpawners();
        gravitySource.gravityEnabled = false;
        player.lockPlayer = true;
        EnableChildren(player.cam, false);
        loadingScreen.ShowLoadingScreen();
        yield return terrainPlacer.GenerateNewTerrain();
        loadingScreen.HideLoadingScreen();
        EnableChildren(player.cam);
        player.lockPlayer = false;
        gravitySource.gravityEnabled = true;
        SpawnEnemy.instance.StartSpawners();
    }

    void EnableChildren(Transform transform, bool active = true)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(active);
        }
    }
}