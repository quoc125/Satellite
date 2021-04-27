using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CanvasController : MonoBehaviour
{
    [SerializeField]
    protected TextMeshProUGUI dateTime;

    public void UpdateDate(string time)
    {
        dateTime.text = time;
    }
}
