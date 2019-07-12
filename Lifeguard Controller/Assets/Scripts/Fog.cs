/*
 * author: Mikel Jauregui
 * last update: 12/07/2019
 * description: Se enecarga de modificar la 
 * escala de cada elemento del fog.
 */

using UnityEngine;

public class Fog : MonoBehaviour {

    [Header("Player:")]
    public float playerRadius;
    public float minDistancePlayer;
    public float minLerpPlayer;
    [Header("Tower:")]
    public float towerRadius;
    public float minDistanceTower;
    public float minLerpTower;
    [Header("Beach:")]
    Transform beach;
    public float maxDistanceBeach;
    public float minDistanceBeach;
    public float minLerpBeach;

    Vector3 scale;

    private void Start() {
        // Guardamos la escala inicial puesto 
        // que sera modificada en un futuro.
        scale = transform.localScale;
    }

    void Update() {
        // Recibimos un valor de 0 a 1 segun
        // la distancia a dichos objetos.
        float playerLerp = GetLerp(playerRadius, minDistancePlayer, minLerpPlayer, LayerMask.GetMask("Detection"));
        float towerLerp = GetLerp(towerRadius, minDistanceTower, minLerpTower, LayerMask.GetMask("Tower"));

        float lerp = Mathf.Min(playerLerp, towerLerp);

        // Modificamos la escala.
        float scaleX = Mathf.Lerp(0, scale.x, lerp);
        float scaleY = Mathf.Lerp(0, scale.y, lerp);
        float scaleZ = Mathf.Lerp(0, scale.z, lerp);
        transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

        // Modificacmos la posicion para que concuerde despues de 
        // la modificacion de la escala.
        Vector3 pos = transform.position;
        pos.y = transform.parent.position.y + scaleY / 2;
        transform.position = pos;
    }

    float GetLerp(float radius, float minDistance, float minLerp, int layerMask) {

        float lerp = 1;
        GameObject nearest = null;

        // Buscamos todos los objetos que consuerden de un radio.
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask);

        foreach (var item in colliders) {
            // Guardamos en nearest el objeto mas cercano.

            if (nearest == null) {
                nearest = item.gameObject;
                continue;
            }
            if (Vector3.SqrMagnitude(nearest.transform.position - transform.position) > Vector3.SqrMagnitude(item.transform.position - transform.position)) {
                nearest = item.gameObject;
            }
        }
        if (nearest != null) {
            // Calculamos un valor de 0 a 1 segun la distancia
            // del objeto.
            float sqDistance = Vector3.SqrMagnitude(nearest.transform.position - transform.position);
            lerp = Mathf.InverseLerp(minDistance, radius * radius, sqDistance);
            lerp = lerp > minLerp ? lerp : 0;
        }
        return lerp;
    }
}
