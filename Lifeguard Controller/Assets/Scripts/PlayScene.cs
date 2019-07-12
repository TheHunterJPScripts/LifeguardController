using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayScene : MonoBehaviour
{
    public int sceneIndex;
    public void Play() {
        SceneManager.LoadScene(sceneIndex);
    }
}
