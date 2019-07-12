using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Player : MonoBehaviour {

    bool isSelected;
    Transform target;
    NavMeshAgent agent;

    public Transform selected;

    public void SetTarget(Transform target) {
        this.target = target;
    }
    public void SetSelected(bool isSelected) {
        this.isSelected = isSelected;
    }


    private void Start() {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        if (target) {
            agent.destination = target.position;
        }
        if (selected != null) {
            if (isSelected) {
                selected.gameObject.SetActive(true);
            } else {
                selected.gameObject.SetActive(false);
            }
        }
    }
}
