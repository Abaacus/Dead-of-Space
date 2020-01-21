using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyOrbiter : MonoBehaviour
{
    public float skySpeed;
    public Material skybox;

    private void FixedUpdate()
    {
        if (skybox.HasProperty("_Rotation"))
        {
            skybox.SetFloat("_Rotation", Time.fixedTime * skySpeed);
        }
    }
}
