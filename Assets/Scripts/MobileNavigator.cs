using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobileNavigator : MonoBehaviour {

    // Navmesh agent component reference
    private NavMeshAgent agent;

    // Movement speeds
    public float wanderSpeed;
    public float pursuitSpeed;

    // Radius of area the enemy roams with in relative to itself
    public float wanderRadius;

    // Radius size of line of sight
    public float losSize;

    // temporary timer
    public float timer = 4;

	// Set component references
	void Start ()
    {
        agent = GetComponent<NavMeshAgent>();

        print(Mathf.DeltaAngle(1080, 90));
        print(Mathf.DeltaAngle(5, 18));
        print(Mathf.DeltaAngle(5, 300));
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(timer > 0)
        {
            //timer -= Time.deltaTime;
        }
        else
        {
            Vector3 position;

            FindRandomPosition(transform.position, wanderRadius, out position);

            agent.SetDestination(position);

            timer = 4.0f;
        }

        Debug.DrawRay(transform.position, Vector3.Normalize(transform.forward + new Vector3( 1, 0, 1)) * 20.0f, Color.blue);
        Debug.DrawRay(transform.position, Vector3.Normalize(transform.forward + new Vector3(-1, 0, 1)) * 20.0f, Color.blue);

        Debug.DrawRay(transform.position, Vector3.Normalize(transform.forward + new Vector3( 1, 0, 0)) * 20.0f, Color.red);
        Debug.DrawRay(transform.position, Vector3.Normalize(transform.forward + new Vector3(-1, 0, 0)) * 20.0f, Color.red);

        if (CheckLOS())
        {
            gameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 1);
        }
        else
        {
            gameObject.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1);
        }
    }

    // Check if the player is within line of sight
    bool CheckLOS()
    {
        //if (Mathf.Abs(Vector3.Angle(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position)) < losSize)
        if (Vector3.Dot(Vector3.Normalize(GameObject.FindGameObjectWithTag("Player").transform.position), Vector3.Normalize(transform.position)) > Mathf.Acos(losSize * Mathf.PI / 180))
        {
            return true;
        }

        return false;
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
}
