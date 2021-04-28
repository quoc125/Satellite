using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    protected const float TOTAL_WAIT = 0.5f;

    [SerializeField]
    protected TextMeshProUGUI dateTime;
    [SerializeField]
    protected Button test;
    private float wait = 0;

    public void UpdateDate(string time)
    {
        dateTime.text = time;
    }

    public void Start()
    {
        test.gameObject.SetActive(false);
    }

    public void Update()
    {
        if (Input.GetAxis("Menu") == 1.0f && wait < 0)
        {
            test.gameObject.SetActive(!test.gameObject.activeSelf);
            wait = TOTAL_WAIT;
        }
        wait -= Time.deltaTime;
    }
}
