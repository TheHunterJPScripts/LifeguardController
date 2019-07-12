using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saves : MonoBehaviour
{
    public static bool dontShow;
    // Start is called before the first frame update
    void Start()
    {
        if(GameObject.Find(this.name).GetInstanceID() != this.GetInstanceID()) {
            GameObject.Destroy(this.gameObject);
        }
        else
            DontDestroyOnLoad(this.gameObject);
    }
}
