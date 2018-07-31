using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StationarySeeker : MonoBehaviour {

    // Player transform reference
    private Transform playerTransform;

    // State: Scanning, Tracking, Checking
    public string state = "Scanning";

    // Checks for sight
    private bool scanReady = true;
    private bool trackReady = true;
    private bool checkReady = true;
    private bool losReady = true;

    // Line of sight movemnt speed
    public float turnSpeed;
    public float scanSpeed;
    public float trackSpeed;

    //Time along turn interpolation
    private float turnTime;

    // Radius of the area the enemy will scan relative to itself
    public float scanRadius;

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
    public int initialfireDelay;
    public int maxFireDelay;
    private int curentFireDelay;

    // Time spent checking last known player position
    public float checkTime;

    // Array of all seeker points in range
    //public Transform[] seekerPoints;

    // Set component reference and initial sight
    void Start ()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        currentPosition = transform.position + transform.forward * 20;

        /*// Find all seeker points in range
        GameObject[] allSeekerPoints = GameObject.FindGameObjectsWithTag("Seeker Point");

        int a = 0;

        for(int i = 0; i < allSeekerPoints.Length; i++)
        {

        }*/
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
        
        Debug.DrawRay(transform.position, currentAngle * 20, Color.blue);
        Debug.DrawRay(transform.position, newAngle * 20, Color.blue);
        Debug.DrawRay(transform.position, sightDirection * 20, Color.red);

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

        FindRandomPosition(transform.position, scanRadius, out position);

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

    // Find random position
    bool FindRandomPosition(Vector3 centre, float radius, out Vector3 result)
    {
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

        result = Vector3.zero;

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

                Instantiate(enemyBullet, bulletSpawn.position, bulletSpawn.rotation);
                //Instantiate(enemyBullet, bulletSpawn.position, bulletSpawn.localRotation);
                //Instantiate(enemyBullet, bulletSpawn.position, Quaternion.Euler(bulletSpawn.forward));
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

        state = "Scanning";
    }
}
