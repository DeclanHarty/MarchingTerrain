using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CaseTile : MonoBehaviour
{
    public TextMeshPro textObject;

    public void SetText(string value)
    {
        textObject.text = value;
    }
}
