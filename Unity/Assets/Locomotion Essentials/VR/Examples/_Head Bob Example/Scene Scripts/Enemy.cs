//          ======= Copyright (c) BlackPandaStudios, All rights reserved. =======
//              _____________________________________________________________ 
// <author>    | -  Joel Gabriel                                             |  
// <date>      | -  05/05/2017                                               |  
// <name>      | -  Enemy.cs                                                 |  
// <summary>   | -  This class handles the logic for the enemy AI in the     |            
//             |    head bob scene. This script is scene specific.           |
//             |_____________________________________________________________|

using Loco.VR;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    NavMeshAgent agent;
    Transform target;

    public float health = 50;
    
	void Start () {
        target = FindObjectOfType<VRLocomotionRig>().transform;
        agent = GetComponent<NavMeshAgent>();
    }
	
	void Update () {
        if (!IsAlive()) RunToPlayer();
        else Die();
    }

    #region Private Functions

    bool IsAlive() { return (health <= 0); }
    void RunToPlayer(){
        if(agent != null) agent.SetDestination(target.position);
    }
    void Die(){
        Destroy(gameObject);
    }

    #endregion
}
