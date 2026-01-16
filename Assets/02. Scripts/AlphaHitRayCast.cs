using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaHitRayCast : MonoBehaviour
{
    public float threshold = 0.3f;
    void Start()
    {
        Image img = GetComponent<Image>();
        if (img != null)
        {
            img.alphaHitTestMinimumThreshold = threshold;
        }
    }

    
}
