using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobileNavigator : MonoBehaviour {

    // Type of mobile enemy: 5 - Bandit, 6 - RedSoldier, 7 - BlackSoldier, 8 - SWAT
    public int type;

    // Navmesh agent component reference
    private NavMeshAgent agent;

    // Player transform reference
    private Transform playerTransform;

    // State: Wandering, Pursuing, Checking, Hurt
    public string state = "Wandering";

    // Checks for movement
    private bool wanderReady = true;
    private bool pursueReady = true;
    private bool checkReady = true;
    private bool hurtReady = true;
    private bool losReady = true;

    // Movement speeds
    public float wanderSpeed;
    public float pursuitSpeed;

    // Random roam radius control
    public float wanderRadiusMax;
    public float wanderRadiusMin;
    public float wanderRadiusRand;

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

    // New direction to turn to after getting hit
    public Vector3 hurtRotation;

    // Array of all wander points
    private GameObject[] allWanderPoints;
    private bool[] allWanderInRange;

    // Set component references and find wander points
    void Start ()
    {
        agent = GetComponent<NavMeshAgent>();

        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        allWanderPoints = GameObject.FindGameObjectsWithTag("Wander Point");
        allWanderInRange = new bool[allWanderPoints.Length];
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
        else if(hurtReady && state == "Hurt")
        {
            StartCoroutine(Hurt());
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

    // Wander to random position
    IEnumerator Wander()
    {
        wanderReady = false;
        
        Vector3 position;
        
        FindRandomPosition(FindRandomWanderPoint(), wanderRadiusRand, out position);

        agent.speed = wanderSpeed;

        agent.stoppingDistance = 0.0f;

        agent.SetDestination(position);
        
        yield return new WaitForSeconds(wanderDelay + Random.Range(-wanderDelayRandom, wanderDelayRandom));

        wanderReady = true;
    }

    // Find random wander point
    Vector3 FindRandomWanderPoint()
    {
        int randMax = 0;

        for (int i = 0; i < allWanderPoints.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, allWanderPoints[i].transform.position);

            if (distance > wanderRadiusMin && distance < wanderRadiusMax)
            {
                allWanderInRange[i] = true;
                
                randMax++;
            }
            else
            {
                allWanderInRange[i] = false;
            }
        }

        int rand = Random.Range(0, randMax) + 1;

        int randCount = 0;

        for (int i = 0; i < allWanderInRange.Length; i++)
        {
            if (allWanderInRange[i])
            {
                randCount++;

                if(randCount >= rand)
                {
                    return allWanderPoints[i].transform.position;
                }
            }
        }
        
        return transform.position;
    }

    // Find random position
    bool FindRandomPosition(Vector3 centre, float radius, out Vector3 result)
    {
        if(Vector3.Distance(centre, transform.position) < wanderRadiusMin)
        {
            radius = wanderRadiusMin / 2;
        }

        for (int i = 0; i < 15; i++)
        {
            Vector3 rand = centre + Random.insideUnitSphere * radius;

            NavMeshHit hit;

            if(NavMesh.SamplePosition(rand, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;

                return true;
            }
        }

        result = transform.position;

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

                GameObject newBullet = Instantiate(enemyBullet, bulletSpawn.position, bulletSpawn.rotation);

                newBullet.GetComponent<BulletController>().type = type;
                newBullet.GetComponent<BulletController>().canHurtPlayer = true;
            }
        }

        yield return new WaitForSeconds(0.01f);

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

    // Turn to direction of damage
    IEnumerator Hurt()
    {
        hurtReady = false;

        agent.SetDestination(transform.position);

        float hurtTurnTime = 0;
        
        Vector3 initialProjected = transform.position + transform.forward * 10;
        Vector3 hurtProjected = transform.position + hurtRotation * 10;

        Vector3 hurtScanDirection = initialProjected;
        
        while(hurtScanDirection != hurtProjected && state == "Hurt")
        {
            hurtScanDirection = Vector3.Slerp(initialProjected, hurtProjected, hurtTurnTime * 5);

            transform.LookAt(new Vector3(hurtScanDirection.x, transform.position.y, hurtScanDirection.z));
            
            Debug.DrawRay(transform.position, initialProjected, Color.red, 10);
            Debug.DrawRay(transform.position, hurtProjected, Color.blue, 10);
            Debug.DrawRay(transform.position, transform.forward * 20, Color.green, 10);

            hurtTurnTime += Time.deltaTime;
            
            yield return new WaitForEndOfFrame();
        }
        
        yield return new WaitForSeconds(checkTime);

        hurtRotation = Vector3.zero;
        
        hurtReady = true;
        
        state = "Wandering";
    }
}
