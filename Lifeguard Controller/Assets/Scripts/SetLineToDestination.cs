using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class SetLineToDestination : MonoBehaviour {
    public Transform target;
    public NavMeshAgent agent;
    LineRenderer lineRenderer;
    public GameObject end;
    Vector3 o;
    // Start is called before the first frame update
    void Start() {
        lineRenderer = GetComponent<LineRenderer>();
        o = transform.position;
    }

    // Update is called once per frame
    void Update() {
        transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
        if (agent.hasPath) {
            Vector3[] pos = new Vector3[agent.path.corners.Length];
            for (int i = 0; i < agent.path.corners.Length; i++) {
                Vector3 v = agent.path.corners[i];
                pos[i] = new Vector3(v.x - transform.position.x , v.z - transform.position.z , 0)/2;
                pos[i].z = 0;
            }

            lineRenderer.positionCount = pos.Length;
            lineRenderer.SetPositions(pos);
            lineRenderer.alignment = LineAlignment.TransformZ;

            end.SetActive(true);
            Vector3 p = agent.path.corners[pos.Length - 1];
            p.y = 0.502f + 0.3f;
            end.transform.position = p;
        } else {
            end.SetActive(false);
        }
    }
}
