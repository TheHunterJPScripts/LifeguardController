using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour {
    [System.Serializable]
    public struct Lvl {
        public int coinGain;
        public int enemyMaxAmount;
        public GameObject[] spawners;
        public GameObject[] shops;
    }

    public enum State { Null, Selecting, Playing, Ended, Lost }
    public State currentState = State.Selecting;
    public State previousState = State.Null;
    public State nextState = State.Null;
    public int days;
    public int minutesPerDay;
    public int coins;
    public Lvl[] objects;
    public List<GameObject> players;
    public List<GameObject> enemies;

    public float time;
    public int fase;
    public bool start;
    public int death;
    public int nextScene = -1;
    public GameObject pauseScreen;
    public GameObject winScreen;
    public GameObject lostMenu;

    private void Update() {
        switch (currentState) {
            case State.Selecting:
                Selecting();
                break;
            case State.Playing:
                Playing();
                break;
            case State.Ended:
                Ended();
                break;
            case State.Lost:
                Lost();
                break;
            default:
                break;
        }
        previousState = currentState;
        currentState = nextState != State.Null ? nextState : currentState;
        nextState = State.Null;
    }

    void Selecting() {
        if (previousState != State.Selecting) {
            // Si se acaba de cambiar de modo
            // activamos todas las tiendas del nivel.
            for (int i = 0; i < objects[fase].shops.Length; i++) {
                objects[fase].shops[i].SetActive(true);
            }
            coins += objects[fase].coinGain;
            pauseScreen.SetActive(true);
        }

        if (start) {
            for (int i = 0; i < objects[fase].shops.Length; i++) {
                objects[fase].shops[i].GetComponent<Shop>().StartLevel();
                objects[fase].shops[i].SetActive(false);
            }
            start = false;
            nextState = State.Playing;
        }
    }


    void Playing() {
        if(death >= 3) {
            nextState = State.Lost;
        }

        if (previousState != State.Playing) {
            for (int i = 0; i < objects[fase].spawners.Length; i++) {
                objects[fase].spawners[i].SetActive(true);
            }
        }

        // Si se ha acabado el tiempo de juego.
        if (time >= minutesPerDay * 60) {
            // Si no quedan mas fases.
            if (fase >= days - 1) {
                // Mostrar los menus que haya que mostrar.
                nextState = State.Ended;
            } else {
                // Mostrar los elementos que haya que mostrar.
                nextState = State.Selecting;
            }
            // Eliminamos a todos los elementos.
            for (int i = 0; i < enemies.Count; i++) {
                GameObject.Destroy(enemies[i]);
            }
            enemies.Clear();
            for (int i = 0; i < players.Count; i++) {
                GameObject.Destroy(players[i]);
            }
            players.Clear();
            // Desactivamos a todos los spawners.
            for (int i = 0; i < objects[fase].spawners.Length; i++) {
                objects[fase].spawners[i].SetActive(false);
            }
            time = 0;
            fase++;
        }
        time += Time.deltaTime;
    }

    void Ended() {
        if(nextScene !=-1)
            SceneManager.LoadScene(nextScene);
        else {
            winScreen.SetActive(true);
        }
    }

    void Lost() {
        if(previousState != currentState) {
            // Eliminamos a todos los elementos.
            enemies.Clear();
            for (int i = 0; i < players.Count; i++) {
                GameObject.Destroy(players[i]);
            }
            players.Clear();
            // Desactivamos a todos los spawners.
            for (int i = 0; i < objects[fase].spawners.Length; i++) {
                objects[fase].spawners[i].SetActive(false);
            }
            // Desactivamos a todos los spawners.
            for (int i = 0; i < objects[fase].spawners.Length; i++) {
                objects[fase].spawners[i].SetActive(false);
            }
            lostMenu.SetActive(true);
        }
    }
    public void StartGame() {
        start = true;
    }
}
