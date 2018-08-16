using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobileNavigator : MonoBehaviour {

    // Type of mobile enemy: 5 - Bandit, 6 - RedSoldier, 7 - BlackSoldier, 8 - SWAT
    public int type;

    // Enemy Animation Controller Reference
    public Animator EnemyAnim;
    private bool mWalking = false;
    private bool mIdle = false;
    private bool mFire = false;

    // Navmesh agent component reference
    private NavMeshAgent agent;

    // Sound control
    private AudioSource mobileAudio;
    public AudioClip[] mobileClips;

    // Player transform reference
    private Transform playerTransform;

    // State: Wandering, Pursuing, Checking, Responding
    public string state = "Wandering";

    // Checks for movement
    private bool wanderReady = true;
    private bool pursueReady = true;
    private bool checkReady = true;
    private bool respondReady = true;
    private bool detectReady = true;

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
    private Vector3 enemySightDirection;

    // Size of area of detection
    public float detectionRadius;

    // Firing range (stopping range of navmesh)
    public float firingRange;

    // Turn rate when pursuing
    public float turnRate;

    // Enemy gunplay
    public GameObject enemyBullet;
    public Transform bulletSpawn;
    public int initialFireDelay;
    public int maxFireDelay;
    private int curentFireDelay;

    // Time spent checking last known player position
    public float checkTime;

    // New direction to turn to after getting hit/player entering aod
    public Vector3 respondRotation;

    // Array of all wander points
    private GameObject[] allWanderPoints;
    private bool[] allWanderInRange;

    // Set component references and find wander points
    void Start ()
    {
        agent = GetComponent<NavMeshAgent>();

        mobileAudio = GetComponent<AudioSource>();

        EnemyAnim = GetComponentInChildren<Animator>();

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

            if (agent.velocity.magnitude > 0)
            {
                mWalking = true;
                mFire = false;
                mIdle = false;
            }
            else
            {
                mWalking = false;
                mFire = true;
                mIdle = false;
            }
        }
        else if(pursueReady && state == "Pursuing")
        {
            StartCoroutine(Pursue());
        }
        else if(checkReady && state == "Checking")
        {
            StartCoroutine(Check());
        }
        else if(respondReady && state == "Responding")
        {
            StartCoroutine(Respond());
        }

        if (detectReady)
        {
            StartCoroutine(DetectionCheck());
        }

        EnemyAnim.SetBool("Walking", mWalking);
        EnemyAnim.SetBool("Idle", mIdle);
        EnemyAnim.SetBool("Fire", mFire);
    }

    // Check for player with line of sight and distance
    IEnumerator DetectionCheck()
    {
        detectReady = false;

        EnemySightDetection();
        
        yield return new WaitForSeconds(0.1f);

        detectReady = true;
    }

    // Check line of sight and area of detection
    void EnemySightDetection()
    {
        if (CheckLOS())
        {
            if (state != "Pursuing")
            {
                curentFireDelay = initialFireDelay;

                mobileAudio.PlayOneShot(mobileClips[0]);
            }

            state = "Pursuing";

            gameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 1);
        }
        else if (CheckAOD())
        {
            state = "Responding";

            respondRotation = playerTransform.position - transform.position;
        }
        else
        {
            if (state == "Pursuing")
            {
                state = "Checking";
            }

            gameObject.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1);
        }
    }

    // Check if the player is within line of sight
    bool CheckLOS()
    {
        Vector3 angleToPlayer = playerTransform.position - transform.position;
        
        if(state == "Pursuing")
        {
            enemySightDirection = playerTransform.position - transform.position;
        }
        else
        {
            enemySightDirection = transform.forward;
        }

        if (Vector3.Angle(angleToPlayer, enemySightDirection) < losAngle)
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
    
    // Check if the player is within area of detection
    bool CheckAOD()
    {
        if(Vector3.Distance(transform.position, playerTransform.position) < detectionRadius)
        {
            Ray ray = new Ray(transform.position, playerTransform.position - transform.position);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, losLength))
            {
                if (hit.collider.tag == "Player")
                {
                    return true;
                }
            }
        }
        
        return false;
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
                curentFireDelay = maxFireDelay;

                GameObject newBullet = Instantiate(enemyBullet, bulletSpawn.position, Quaternion.LookRotation(playerTransform.position - bulletSpawn.position));

                newBullet.GetComponent<BulletController>().type = type;
                newBullet.GetComponent<BulletController>().canHurtPlayer = true;

                mobileAudio.PlayOneShot(mobileClips[1]);

                if(type == 7)
                {
                    for(int i = 0; i < 5; i++)
                    {
                        newBullet = Instantiate(enemyBullet, bulletSpawn.position, Quaternion.LookRotation(playerTransform.position - bulletSpawn.position)
                            * Quaternion.Euler(Random.Range(-3.25f, 3.25f), Random.Range(-3.25f, 3.25f), Random.Range(-3.25f, 3.25f)));

                        newBullet.GetComponent<BulletController>().type = type;
                        newBullet.GetComponent<BulletController>().canHurtPlayer = true;
                    }
                }
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

        if(Vector3.Magnitude(agent.velocity) > 0.25f)
        {
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(checkTime);

        checkReady = true;

        if(state == "Checking")
        {
            state = "Wandering";
        }
    }

    // Turn to direction of damage
    IEnumerator Respond()
    {
        respondReady = false;
        
        agent.SetDestination(transform.position);

        float respondTurnTime = 0;
        
        Vector3 initialProjected = transform.position + transform.forward * 10;
        Vector3 respondProjected = transform.position + respondRotation * 10;

        Vector3 respondScanDirection = initialProjected;
        
        while(respondScanDirection != respondProjected && state == "Responding")
        {
            respondScanDirection = Vector3.Slerp(initialProjected, respondProjected, respondTurnTime * 5);

            transform.LookAt(new Vector3(respondScanDirection.x, transform.position.y, respondScanDirection.z));
            
            /*Debug.DrawRay(transform.position, initialProjected, Color.red, 10);
            Debug.DrawRay(transform.position, respondProjected, Color.blue, 10);
            Debug.DrawRay(transform.position, transform.forward * 20, Color.green, 10);*/

            respondTurnTime += Time.deltaTime;
            
            yield return new WaitForEndOfFrame();
        }
        
        yield return new WaitForSeconds(checkTime);

        respondRotation = Vector3.zero;
        
        respondReady = true;
        
        if(state == "Responding")
        {
            state = "Wandering";
        }
    }
}
