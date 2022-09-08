using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // in my case the enemy is the spiders
    [SerializeField]
    private float timeOut = 5f; // time to wait after impact, in seconds
    [SerializeField]
    private int maxHits = 5; // after this number of hits, make it stop so player can escape
    private NavMeshAgent agent;
    [SerializeField]
    private int damage = -10;
    public int Damage { get { return damage; } }
    [SerializeField]
    private GameObject player;

    private bool follow = false;
    private float timeWaiting = 0f;
    private int hits = 0; // how many times we hit already
    private bool onTimeout = false; // this will be true when we are activly waiting to hit again
    private Animator animator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (onTimeout)
        {
            if (timeWaiting <= 0) // timeout is over 
                onTimeout = false;
            else
            {
                timeWaiting -= Time.deltaTime;
            }
        }
        else
        {
            if (follow)
            {
                FollowPlayer();

            }
            if(animator.GetBool("Walk")!= follow)
                animator.SetBool("Walk", follow);

        }
    }
    private void FollowPlayer()
    {
        if(onTimeout)
        {
            follow = false;
        }
        agent.SetDestination(player.transform.position);
    }
    private void HitPlayer() // on impact 
    {
        if (!onTimeout) // if we can hit 
        {
            Debug.Log("spider hit player ");
            // stop the enemy from moving 
            agent.SetDestination(transform.position);
            // send out event that a collision has occurred 
            CustomEventSystem.Instance.OnEvent(EventType.EnemyCollision, this.gameObject);
            // make the enemy stop for some time
            onTimeout = true;
            timeWaiting = timeOut;
            hits++;
            animator.SetTrigger("Attack");
        }
    }
    // collision tells us when we actually hit the player
    // trigger tell us when we are in range
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(hits < maxHits)
            {
                HitPlayer();
            }
            else if (hits == maxHits)
            {
                follow = false;
                animator.SetBool("Death", true);
            }
        }
    }


    // Enemies chase us based on distance - when the player is in distance from the spider
    // send  the trigger to start following it 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Debug.Log("player is in distance from spider");
            follow = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        follow = false;
    }

}
