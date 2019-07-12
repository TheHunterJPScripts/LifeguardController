using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class MoneyTracker : MonoBehaviour
{
    public Level level;
    public TextMeshProUGUI text;
    private void Start() {
        text = GetComponent<TextMeshProUGUI>();
    }
    // Update is called once per frame
    void Update()
    {
        text.text = level.coins.ToString();
    }
}
