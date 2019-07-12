using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class Traker : MonoBehaviour {
    public enum MouseButtons { Left, Right, Middle }


    [Header("Spawn Platform: ")]
    public Vector3 spawnPlatformPos;
    public Vector2 spawnPlatformSize;


    [Space(10)]
    public MouseButtons selectButton;
    public GraphicRaycaster grRay;
    public EventSystem evSystem;

    public GameObject prefab;
    public float position;
    public GameObject obj;
    Camera cam;
    Vector3 center;

    void Start() {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        //Creamos el nuevo Pointer Event.
        PointerEventData pointData = new PointerEventData(evSystem);
        // Le asignamos la posicion del raton.
        pointData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        grRay.Raycast(pointData, results);

        // Si no estamos haciendo click en algun menu o boton.
        bool outUI = results.Count == 0 && !Physics.Raycast(ray, Mathf.Infinity, LayerMask.GetMask("UI"));
        if (outUI) {
            // Left CLick.
            if (Input.GetMouseButtonDown(0)) {
                // Seleccionamos  el objeto al que apuntamos si es que
                // existe.
                RaycastHit playerPoint = new RaycastHit();
                if (Physics.Raycast(ray, out playerPoint, Mathf.Infinity, LayerMask.GetMask("Detection"))) {
                    if (obj != null)
                        obj.transform.parent.GetComponent<Player>().SetSelected(false);
                    obj = playerPoint.collider.gameObject;
                    obj.transform.parent.GetComponent<Player>().SetSelected(true);
                } else {
                    if (obj != null)
                        obj.transform.parent.GetComponent<Player>().SetSelected(false);
                    obj = null;
                }

            }

            // Right CLick.
            if (Input.GetMouseButtonDown(1)) {
                if (obj != null) {
                    // Lanzamos un raycast para mirar si se esta apuntando
                    // a una persona.
                    RaycastHit targetPoint = new RaycastHit();
                    if (Physics.Raycast(ray, out targetPoint, Mathf.Infinity, LayerMask.GetMask("Target"))) {
                        // Asignamos el destino.
                        obj.transform.parent.GetComponent<Player>().SetTarget(targetPoint.transform);
                    } else {
                        // Lanzamos un raycast para buscar si hay terreno donde se apunta.
                        RaycastHit terrainPoint = new RaycastHit();
                        if (Physics.Raycast(ray, out terrainPoint, Mathf.Infinity, LayerMask.GetMask("Terrain"))) {
                            // Asignamos el destino.
                            obj.transform.parent.GetComponent<NavMeshAgent>().destination = terrainPoint.point;
                            obj.transform.parent.GetComponent<Player>().SetTarget(null);
                        }
                    }
                }
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(spawnPlatformPos, new Vector3(spawnPlatformSize.x, 0, spawnPlatformSize.y));
    }
}
