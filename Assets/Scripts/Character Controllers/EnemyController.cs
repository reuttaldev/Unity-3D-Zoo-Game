using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // in my case the enemy is the spiders
    [SerializeField]
    private float timeOut = 5f; // time to wait after impact, in seconds
    [SerializeField]
    private NavMeshAgent agent;
    [SerializeField]
    private int damage = -10;
    public int Damage { get { return damage; } }
    private bool read = false;
    private PlayerHealth playerHealth;
    private float timeWaiting = 0f;
    private bool onTimeout = false; // this will be true when we are activly waiting to hit again

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        if (onTimeout)
        {
            if (timeWaiting <= 0) // timeout is over 
            {
                onTimeout = false;
                FollowPlayer();
            }
            else
            {
                timeWaiting -= Time.deltaTime;

            }
        }
    }
    private void FollowPlayer()
    {

        agent.SetDestination(playerHealth.gameObject.transform.position);
    }


    private void HitPlayer() // on impact 
    {
        if (!onTimeout) // if we can hit 
        {
            // stop the enemy from moving 
            agent.SetDestination(transform.position);
            // send out event that a collision has occurred 
            EventSystem.Instance.OnEvent(EventType.EnemyCollision, this.gameObject);
            // make the enemy stop for some time
            onTimeout = true;
            timeWaiting = timeOut;
        }
    }
    // collision tells us when we actually hit the player
    // trigger tell us when we are in range
    private void OnCollisionEnter(Collider other)
    {
        HitPlayer();
    }

    // Enemies chase us based on distance - when the player is in distance from the spider
    // send  the trigger to start following it 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !read)
        {
            Debug.Log("player is in distance from spider");
            // trigger objective completion
            read = true;
            FollowPlayer();
        }
    }

}
