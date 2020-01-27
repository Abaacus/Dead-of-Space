using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
    }

    public void LoadScene(int sceneIndex = 1)
    {
        StartCoroutine(ILoadScene(sceneIndex));
    }

    IEnumerator ILoadScene(int sceneIndex)
    {
        Debug.Log("Waiting");
        SceneTransitor.instance.LoadOut();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneIndex);
    }
}
