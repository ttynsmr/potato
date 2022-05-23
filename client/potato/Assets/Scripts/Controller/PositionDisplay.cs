using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionDisplay : MonoBehaviour
{
    public TextMesh text;

    void Update()
    {
        text.text = $"({transform.position.x},{transform.position.y},{transform.position.z})";
    }
}
