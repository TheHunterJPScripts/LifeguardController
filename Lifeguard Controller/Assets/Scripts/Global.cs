/*
 * author: Mikel Jauregui
 * last update: 12/07/2019
 * description: Se encarga del ciclo dia/noche y 
 * contiene una funcion que devuelve la posicion para moverse.(usada
 * por la IA).
 */

using UnityEngine;

public class Global : MonoBehaviour
{
    public Level level;
    public Transform lightSource;
    public Transform[] terrains;
    public Transform[] waters;
    
    public Vector3 GetPosition(bool wantToSwim) {
        // Escogemos aleatoriamente la plataforma a la que tiene que moverse.
        Transform selected;
        if (wantToSwim) {
            int index = Random.Range(0, waters.Length-1);
            selected = waters[index];
        } else {
            int index = Random.Range(0, terrains.Length - 1);
            selected = terrains[index];
        }
        // Dentro de esa plataforma escogemos una posicion aleatoria.
        float sizeX = selected.lossyScale.x;
        float sizeZ = selected.lossyScale.z;
        float x = Random.Range(selected.position.x - sizeX/2, selected.position.x + sizeX/2);
        float y = selected.position.y;
        float z = Random.Range(selected.position.z - sizeZ/2, selected.position.z + sizeZ/2);

        return new Vector3(x,y,z);
    }

    private void Update() {
        // Rota la luz para realizar el ciclo dia/noche.
        float lerp = Mathf.InverseLerp(0, level.minutesPerDay*60, level.time);
        float angle = Mathf.Lerp(0, 180, lerp);
        lightSource.rotation = Quaternion.Euler(angle, -150,0);
    }
}
