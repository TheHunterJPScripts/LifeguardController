using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Shop : MonoBehaviour {
    Level level;
    public GameObject prefab;
    public int cost;
    public bool bought;
    public GameObject button;

    private void Start() {
        level = GameObject.Find("Level").GetComponent<Level>();
    }
    private void Update() {
        button.SetActive(!bought);
    }


    public void Buy() {
        if (level.coins >= cost) {
            level.coins -= cost;
            bought = true;
        }
    }
    public void StartLevel() {
        if (bought) {
            GameObject obj = Instantiate(prefab);
            obj.transform.position = transform.position;
            obj.transform.rotation = this.transform.rotation;
            level.players.Add(obj);
        }
    }
}
