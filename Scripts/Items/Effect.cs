using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public float effectLength;

    private void Start()
    {
        StartCoroutine(EffectDecay());
    }

    IEnumerator EffectDecay()
    {
        yield return new WaitForSeconds(effectLength);
        Destroy(gameObject);
    }
}
