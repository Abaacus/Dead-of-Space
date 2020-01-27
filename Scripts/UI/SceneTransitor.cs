using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitor : MonoBehaviour
{
    public static SceneTransitor instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Multiple instances of " + this + " found");
        }

        instance = this;
    }

    public Animator transitionAnimator;

    private void Start()
    {
        LoadIn();
    }

    public Coroutine LoadIn()
    {
        transitionAnimator.Play("LoadIn");
        return StartCoroutine(WaitForAnimation());
    }

    public Coroutine LoadOut()
    {
        transitionAnimator.Play("LoadOut");
        return StartCoroutine(WaitForAnimation());
    }

    IEnumerator WaitForAnimation()
    {
        yield return new WaitForSeconds(transitionAnimator.GetCurrentAnimatorStateInfo(0).length - transitionAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
    }
}
