using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowLabel : MonoBehaviour
{

    public GameObject target;
    Rect rect = new Rect(0, 0, 300, 100);
    Vector3 offset = new Vector3(0f, 0f, 0); // height above the target position

    // Start is called before the first frame update
    void OnGUI()
    {
        Vector3 point = Camera.main.WorldToScreenPoint(gameObject.transform.position + offset);
        rect.x = point.x;
        rect.y = Screen.height - point.y - rect.height; // bottom left corner set to the 3D point
        if (Math.Abs(Vector3.Distance(Camera.main.transform.parent.position, gameObject.transform.position)) < 30)
        {
            GUI.Label(rect, target.name); // display its name, or other string
        }

    }

}
