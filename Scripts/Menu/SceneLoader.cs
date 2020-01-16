using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadSceneAsync(int sceneIndex = 1)
    {
        SceneManager.LoadSceneAsync(sceneIndex);
    }
}
