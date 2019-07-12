/*
 * author: Mikel Jauregui 
 * last update: 12/07/2019
 * description: Se encarga del movimiento de
 * la camara.
 */

using UnityEngine;

public class CameraMovement : MonoBehaviour {
    public bool isStatic;
    public Vector2 size;
    public Vector3 center;
    public Level level;

    void Update() {
        // Si tenemos seleccionado que sea estatica
        // no queremos que esta funcion haga nada.
        if (isStatic)
            return;

        // Sacamos los valores del input.
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Multiplicamos el vector del input con la rotacion de la camara
        // para que forward sea en la direccion donde mira la camara.
        Vector3 v = Quaternion.Euler(0, this.transform.rotation.eulerAngles.y, 0) * new Vector3(x, 0, z);

        // Calculamos la nueva posicion y nos aseguramos de que no se salga del area designado.
        float X = v.x + transform.position.x;
        float clampX = Mathf.Clamp(X, center.x - size.x / 2, center.x + size.x / 2);

        float Z = v.z + transform.position.z;
        float clampZ = Mathf.Clamp(Z, center.z - size.y / 2, center.z+ size.y/ 2);

        // Aplicamos la nueva posicion.
        transform.position = new Vector3(clampX, transform.position.y, clampZ);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, new Vector3(size.x, 1, size.y));
    }
}
