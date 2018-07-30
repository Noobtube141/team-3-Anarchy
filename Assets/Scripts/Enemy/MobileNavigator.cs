using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobileNavigator : MonoBehaviour {

    // Navmesh agent component reference
    private NavMeshAgent agent;

    // Player transform reference
    private Transform playerTransform;

    // State: Wandering, Pursuing and Checking
    public string state = "Wandering";

    // Checks for movement
    private bool wanderReady = true;
    private bool pursueReady = true;
    private bool checkReady = true;
    private bool losReady = true;

    // Movement speeds
    public float wanderSpeed;
    public float pursuitSpeed;

    // Radius of area the enemy roams within relative to itself
    public float wanderRadius;

    // Delay between wandering
    public float wanderDelay;
    public float wanderDelayRandom;

    // Size of line of sight (radius refers to half the size)
    public float losAngle;
    public float losLength;

    // Firing range (stopping range of navmesh)
    public float firingRange;

    // Turn rate when pursuing
    public float turnRate;

    // Enemy gunplay
    public GameObject enemyBullet;
    public Transform bulletSpawn;
    public int InitialfireDelay;
    public int MaxFireDelay;
    private int curentFireDelay;

    // Time spent checking last known player position
    public float checkTime;
    
    // Set component references
    void Start ()
    {
        agent = GetComponent<NavMeshAgent>();

        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
	}
	
	// Control state behaviour and check los
	void Update ()
    {
        if (wanderReady && state == "Wandering")
        {
            StartCoroutine(Wander());
        }
        else if(pursueReady && state == "Pursuing")
        {
            StartCoroutine(Pursue());
        }
        else if(checkReady && state == "Checking")
        {
            StartCoroutine(Check());
        }

        if (losReady)
        {
            StartCoroutine(EnemyLOS());
        }
    }

    // Check if the player is within line of sight
    bool CheckLOS()
    {
        Vector3 angleToPlayer = playerTransform.position - transform.position;

        if (Vector3.Angle(angleToPlayer, transform.forward) < losAngle)
        {
            Ray ray = new Ray(transform.position, angleToPlayer);

            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, losLength))
            {
                if(hit.collider.tag == "Player")
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Check line of sight
    IEnumerator EnemyLOS()
    {
        losReady = false;

        if (CheckLOS())
        {
            if(state != "Pursuing")
            {
                curentFireDelay = InitialfireDelay;
            }

            state = "Pursuing";

            gameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 1);
        }
        else
        {
            if (state == "Pursuing")
            {
                state = "Checking";
            }

            gameObject.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1);
        }

        yield return new WaitForSeconds(0.1f);

        losReady = true;
    }

    // Call for wndering
    IEnumerator Wander()
    {
        wanderReady = false;

        Vector3 position;

        FindRandomPosition(transform.position, wanderRadius, out position);

        agent.speed = wanderSpeed;

        agent.stoppingDistance = 0.0f;

        agent.SetDestination(position);
        
        yield return new WaitForSeconds(wanderDelay + Random.Range(-wanderDelayRandom, wanderDelayRandom));

        wanderReady = true;
    }

    // Find random position
    bool FindRandomPosition(Vector3 centre, float radius, out Vector3 result)
    {
        for(int i = 0; i < 15; i++)
        {
            Vector3 rand = centre + Random.insideUnitSphere * radius;

            NavMeshHit hit;

            if(NavMesh.SamplePosition(rand, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;

                return true;
            }
        }

        result = Vector3.zero;

        return false;
    }

    // Move towards player position
    IEnumerator Pursue()
    {
        pursueReady = false;

        transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));

        agent.speed = pursuitSpeed;

        agent.stoppingDistance = firingRange;

        agent.SetDestination(playerTransform.position);

        if (Vector3.Magnitude(agent.velocity) < 0.1f)
        {
            curentFireDelay--;

            if (curentFireDelay < 1)
            {
                curentFireDelay = MaxFireDelay;

                Instantiate(enemyBullet, bulletSpawn.position, bulletSpawn.rotation);
            }
        }

        yield return new WaitForSeconds(0.1f);

        pursueReady = true;
    }

    // Check last known player position. If not found, return to wandering state
    IEnumerator Check()
    {
        checkReady = false;

        agent.speed = pursuitSpeed;

        agent.stoppingDistance = 0.0f;

        if(Vector3.Magnitude(agent.velocity) > 0.1f)
        {
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(checkTime);

        checkReady = true;

        state = "Wandering";
    }
}
