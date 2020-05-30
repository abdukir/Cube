using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public Transform cube;
    public TextMeshProUGUI text;

    // Update is called once per frame
    void Update()
    {
        if(cube.transform.position.y >= 0)
        {
            text.text = cube.transform.position.y.ToString("0");
        }
    }
}
