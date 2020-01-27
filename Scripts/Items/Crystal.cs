using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    GravityBody gb;

    public float rotSpeed = 1f;
    public float bobStrength = 1f;
    public float bobSpeed = 1f;

    public GameObject pickupEffect;

    MeshRenderer mr;
    Collider cldr;

    private void Start()
    {
        gb = new GravityBody(GetComponent<Rigidbody>(), SpinType.axis);
        mr = GetComponent<MeshRenderer>();
        cldr = GetComponent<Collider>();

        StartCoroutine(DelayedSpawn());
    }

    private void Update()
    {
        gb.Spin(rotSpeed);
        gb.Elevate(Mathf.Sin(Time.time * bobSpeed) * bobStrength);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Crystal picked up");
            Player.instance.UpdateCrystalCounter(1);
            GravitySource.instance.RemoveGravityObject(gb);
            Instantiate(pickupEffect, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    IEnumerator DelayedSpawn()
    {
        mr.enabled = false;
        cldr.enabled = false;
        yield return new WaitForSeconds(0.2f);
        mr.enabled = true;
        cldr.enabled = true;
    }
}
