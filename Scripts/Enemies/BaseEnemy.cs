using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum EnemyState { passive, targeting, dead };

[RequireComponent(typeof(Rigidbody))]
public class BaseEnemy : MonoBehaviour
{
    [HideInInspector]
    public TerrainPlacer terrainPlacer;
    [HideInInspector]
    public Transform player;
    EnemyState state;
    internal GravityBody gb;

    public float detectionRadius;
    public float rotSpeed = 45;
    public float moveSpeed = 10;
    public float rateOfAscension = 1;
    Vector3 target;

    public GameObject deathAffect;

    public void Start()
    {
        terrainPlacer = TerrainPlacer.instance;
        player = Player.instance.transform;
    }

    void Update()
    {
        UpdatePlayerDistance();

        switch (state)
        {
            default:
            case EnemyState.passive:
                Wander();
                break;

            case EnemyState.targeting:
                Target();
                break;

            case EnemyState.dead:
                Dead();
                break;
        }
    }

    public virtual void Wander()
    {

    }

    public virtual void Target()
    {

    }

    public virtual void Dead()
    {
        // other code goes here

        GravitySource.instance.RemoveGravityObject(gb);
        Instantiate(deathAffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    public void Hit()
    {
        state = EnemyState.dead;
    }

    void UpdatePlayerDistance()
    {
        if (Vector3.Distance(transform.position, player.position) <= detectionRadius)
        {
            //state = EnemyState.targeting;
            target = player.position;
        }
        else
        {
            state = EnemyState.passive;
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
