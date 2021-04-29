using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zeptomoby.OrbitTools;

public class BaseSatelliteModel : MonoBehaviour
{
    public bool ActiveSatelliteModel = false;

    public DateTime ReferenceTime
    {
        get
        {
            return referenceTime;
        }
        set
        {
            referenceTime = value;
            if (referenceTime >= startTime)
            {
                gameObject.SetActive(true);
                EciTime state = satelliteInfo.PositionEci(referenceTime);
                transform.localPosition = new Vector3((float)(state.Position.X / SatelliteManager.KM_TO_UNITY_UNITY),
                    (float)(state.Position.Y / SatelliteManager.KM_TO_UNITY_UNITY),
                    (float)(state.Position.Z / SatelliteManager.KM_TO_UNITY_UNITY));
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    public List<string> Categories
    {
        get
        {
            return categories;
        }
    }


    protected DateTime referenceTime = new DateTime(1970, 1, 1, 0, 0, 0);
    protected DateTime startTime;
    protected List<string> categories;
    public Satellite satelliteInfo;
    public Material inactive;

    public void Initialize(Tle info, string category)
    {
        satelliteInfo = new Satellite(info);
        categories = new List<string>();
        categories.Add(category);
        startTime = info.EpochJulian.ToTime();
        transform.localEulerAngles = Vector3.zero;
        gameObject.name = info.Name;
        if (!ActiveSatelliteModel)
        {
            Material material = gameObject.GetComponent<MeshRenderer>().material = inactive;
        }
    }
}
