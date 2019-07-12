/*
 * author: Mikel Jauregui
 * last update: 12/07/2019
 * description: Activa y desactiva el objeto seleccionado
 * segun la variable guardada en Saves.
 */

using UnityEngine;

public class DontShow : MonoBehaviour
{
    public GameObject hide;

    private void Start() {
        // Desactivamos el objeto segun el estado 
        // de la variable guardada.
        hide.SetActive(!Saves.dontShow);
    }

    public void Toggle(bool show) {
        Saves.dontShow = show;
    }
}
