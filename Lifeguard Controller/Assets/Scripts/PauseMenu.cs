using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject menu;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape)) {
            Time.timeScale = 0;
            Time.fixedDeltaTime = 0;
            menu.SetActive(true);
        }
    }

    public void Resume() {
        Time.timeScale = 1;
        Time.fixedDeltaTime = 1;
    }
}
