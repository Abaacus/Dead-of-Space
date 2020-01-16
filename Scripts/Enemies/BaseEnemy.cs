using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum EnemyState { passive, targeting, dead };

[RequireComponent(typeof(Rigidbody))]
public class BaseEnemy : MonoBehaviour
{
    public static PlanetData planetData;

    Transform player;
    EnemyState state;
    internal GravityBody gb;

    public float detectionRadius;
    public float rotSpeed = 45;
    public float moveSpeed = 10;
    public float verticalSpeed = 1;
    Vector3 target;

    public void Start()
    {
        player = Player.instance.transform;
    }

    void FixedUpdate()
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

        Destroy(gameObject);
    }

    void UpdatePlayerDistance()
    {
        if (Vector3.Distance(transform.position, player.position) <= detectionRadius)
        {
            state = EnemyState.targeting;
            target = player.position;
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
