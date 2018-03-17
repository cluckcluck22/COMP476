// MoveTo.cs
    using UnityEngine;
    using System.Collections;
    
    public class GoTo : MonoBehaviour {
       
       public Transform goal;
       
       void Start () {
          UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
          agent.destination = goal.position; 
       }
    }