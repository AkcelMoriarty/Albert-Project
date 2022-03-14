using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class IAScript : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject target;
    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.enabled)
            agent.SetDestination(target.transform.position);
    }
    public void SetEnabledAgent(bool enabled)
    {
        agent.updatePosition = agent.updateRotation = enabled;
    }
}
