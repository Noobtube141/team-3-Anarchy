using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StationarySeeker : MonoBehaviour {

    // Type of mobile enemy: 9 - Sniper, 10 - Boss
    public int type;

    // Player transform reference
    private Transform playerTransform;

    // Sound control
    private AudioSource stationaryAudio;

    // State: Scanning, Tracking, Checking, Responding
    public string state = "Scanning";

    // Checks for sight
    private bool scanReady = true;
    private bool trackReady = true;
    private bool checkReady = true;
    private bool respondReady = true;
    private bool losReady = true;

    // Line of sight movemnt speed
    public float turnSpeed;
    public float scanSpeed;
    public float trackSpeed;

    //Time along turn interpolation
    private float turnTime;

    // Random scan radius control
    public float scanRadiusMax;
    public float scanRadiusMin;
    public float scanRadiusRand;

    // Delay between scanning
    public float scanDelay;
    public float scanDelayRandom;

    // Size of line of sight (radius refers to half the size)
    public float losAngle;
    public float losLength;

    // Vectors for scanning
    private Vector3 currentPosition;
    private Vector3 newPosition;
    private Vector3 currentAngle;
    private Vector3 newAngle;
    private Vector3 sightDirection;

    // Enemy gunplay
    public GameObject enemyBullet;
    public Transform bulletSpawn;
    public Transform[] bulletSources;
    public int initialfireDelay;
    public int maxFireDelay;
    private int curentFireDelay;

    // Time spent checking last known player position
    public float checkTime;

    // New direction to turn to after getting hit
    public Vector3 respondRotation;

    // Array of all wander points
    private GameObject[] allSeekerPoints;
    private bool[] allSeekerInRange;

    // Set component reference, initial sight and find seeker points
    void Start ()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        stationaryAudio = GetComponent<AudioSource>();

        currentPosition = transform.position + transform.forward * 20;

        allSeekerPoints = GameObject.FindGameObjectsWithTag("Seeker Point");
        allSeekerInRange = new bool[allSeekerPoints.Length];
    }

    // Control state behaviour and check los
    void Update()
    {
        if (scanReady && state == "Scanning")
        {
            StartCoroutine(Scan());
        }
        else if (trackReady && state == "Tracking")
        {
            StartCoroutine(Track());
        }
        else if (checkReady && state == "Checking")
        {
            StartCoroutine(Check());
        }
        else if(respondReady && state == "Responding")
        {
            StartCoroutine(Respond());
        }

        if (losReady)
        {
            StartCoroutine(EnemyLOS());
        }

        LookTowardPoint();
    }

    // Slerp sight between two points
    void LookTowardPoint()
    {
        sightDirection = Vector3.Slerp(currentAngle, newAngle, turnTime * turnSpeed);

        transform.LookAt(new Vector3(sightDirection.x * 20, transform.position.y, sightDirection.z * 20));
        
        /*Debug.DrawRay(transform.position, currentAngle * 20, Color.yellow);
        Debug.DrawRay(transform.position, newAngle * 20, Color.blue);
        Debug.DrawRay(transform.position, sightDirection * 20, Color.red);*/

        turnTime += Time.deltaTime;
    }

    // Check if the player is within line of sight
    bool CheckLOS()
    {
        Vector3 angleToPlayer = playerTransform.position - transform.position;

        if (Vector3.Angle(angleToPlayer, sightDirection) < losAngle)
        {
            Ray ray = new Ray(transform.position, angleToPlayer);

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

    // Check line of sight
    IEnumerator EnemyLOS()
    {
        losReady = false;

        if (CheckLOS())
        {
            if (state != "Tracking")
            {
                curentFireDelay = initialfireDelay;
            }

            state = "Tracking";

            turnSpeed = trackSpeed;

            gameObject.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
        }
        else
        {
            if (state == "Tracking")
            {
                state = "Checking";
            }
            if(state == "Scanning")
            {
                turnSpeed = scanSpeed;
            }

            gameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 1);
        }

        yield return new WaitForSeconds(0.1f);

        losReady = true;
    }

    // Find a new point to look towards
    IEnumerator Scan()
    {
        scanReady = false;

        Vector3 position;

        FindRandomPosition(FindRandomSeekerPoint(), scanRadiusRand, out position);

        if(position.magnitude > 0.0f)
        {
            currentPosition = newPosition;

            newPosition = position;

            currentAngle = currentPosition - transform.position;

            newAngle = newPosition - transform.position;

            turnTime = 0.0f;
        }
        
        yield return new WaitForSeconds(scanDelay + Random.Range(-scanDelayRandom, scanDelayRandom));

        scanReady = true;
    }

    // Find random seeker point
    Vector3 FindRandomSeekerPoint()
    {
        int randMax = 0;

        for (int i = 0; i < allSeekerPoints.Length; i++)
        {
            float newAngle = Vector3.Angle(allSeekerPoints[i].transform.position - transform.position, sightDirection);

            if (newAngle > scanRadiusMin && newAngle < scanRadiusMax)
            {
                allSeekerInRange[i] = true;
                
                randMax++;
            }
            else
            {
                allSeekerInRange[i] = false;
            }
        }

        int rand = Random.Range(0, randMax) + 1;

        int randCount = 0;

        for (int i = 0; i < allSeekerInRange.Length; i++)
        {
            if (allSeekerInRange[i])
            {
                randCount++;

                if (randCount >= rand)
                {
                    return allSeekerPoints[i].transform.position;
                }
            }
        }

        return new Vector3(transform.forward.x, 0, transform.forward.z) * 15;
    }

    // Find random position
    bool FindRandomPosition(Vector3 centre, float radius, out Vector3 result)
    {
        if (Vector3.Angle(centre - transform.position, sightDirection) < scanRadiusMin)
        {
            radius = scanRadiusMin / 2;
        }

        for (int i = 0; i < 15; i++)
        {
            Vector3 rand = centre + Random.insideUnitSphere * radius;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(rand, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;

                return true;
            }
        }

        result = new Vector3(transform.forward.x, 0, transform.forward.z) * 15;

        return false;
    }

    // Track the player's position
    IEnumerator Track()
    {
        trackReady = false;

        transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));

        currentPosition = newPosition;

        newPosition = playerTransform.position;

        currentAngle = currentPosition - transform.position;

        newAngle = newPosition - transform.position;

        turnTime = 0.0f;

        if (sightDirection == newAngle)
        {
            curentFireDelay--;

            if (curentFireDelay < 1)
            {
                curentFireDelay = maxFireDelay;

                stationaryAudio.PlayOneShot(stationaryAudio.clip);

                foreach (Transform source in bulletSources)
                {
                    GameObject newBullet = Instantiate(enemyBullet, source.position, Quaternion.LookRotation(playerTransform.position - bulletSpawn.position));

                    newBullet.GetComponent<BulletController>().type = type;
                    newBullet.GetComponent<BulletController>().canHurtPlayer = true;
                }
            }
        }

        yield return new WaitForSeconds(0.1f);

        trackReady = true;
    }

    // Look at the player's last known position
    IEnumerator Check()
    {
        checkReady = false;
        
        if (sightDirection != newAngle)
        {
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(checkTime);

        checkReady = true;

        if (state == "Checking")
        {
            state = "Scanning";
        }
    }

    // Set new scan position relative to source of damage
    IEnumerator Respond()
    {
        respondReady = false;
        
        Ray firstRay = new Ray(transform.position, sightDirection);

        RaycastHit firstHit;

        if (Physics.Raycast(firstRay, out firstHit))
        {
            currentPosition = firstHit.point;
        }

        Ray respondRay = new Ray(transform.position, respondRotation);

        RaycastHit respondHit;

        if(Physics.Raycast(respondRay, out respondHit))
        {
            newPosition = respondHit.point;
        }
        
        currentAngle = currentPosition - transform.position;

        newAngle = newPosition - transform.position;

        turnTime = 0.0f;

        if (sightDirection != newAngle)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(checkTime);

        respondReady = true;

        if(state == "Responding")
        {
            state = "Scanning";
        }
    }
}
