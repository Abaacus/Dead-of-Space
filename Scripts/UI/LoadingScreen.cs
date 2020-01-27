using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Multiple instances of " + this + " found");
        }

        instance = this;
    }

    public Transform starlight;
    Quaternion starRotation;

    public void ShowLoadingScreen()
    {
        starRotation = starlight.transform.localRotation;
        starlight.transform.localRotation = Quaternion.Euler(100, -180, 0);

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void HideLoadingScreen()
    {
        starlight.transform.localRotation = starRotation;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
