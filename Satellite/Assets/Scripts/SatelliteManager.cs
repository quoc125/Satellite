using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zeptomoby.OrbitTools;

public class SatelliteManager : MonoBehaviour
{
    #region Constants
    public const double EARTH_ROTATION_SPEED_PER_SECOND = 360.0 / 24.0 / 60.0 / 60.0;
    public const float KM_TO_UNITY_UNITY = 158.925f;
    #endregion

    #region Variable
    protected int SplitValue = 1;

    protected DateTime referenceTime = new DateTime(1970, 1, 1, 0, 0, 0);
    protected DateTime simTime;

    [SerializeField]
    public float timeScaling = 1;

    [SerializeField]
    protected Transform baseCoordinateSystem;
    [SerializeField]
    protected EarthModel earth;
    [SerializeField]
    protected GameObject satellitePrefab;
    protected Dictionary<string, BaseSatelliteModel> satelliteModels;
    protected Dictionary<string, List<BaseSatelliteModel>> categories;

    [SerializeField]
    protected string categoriesPath;

    protected List<BaseSatelliteModel>[] splitUpdate;
    [SerializeField]
    protected int DivisionValue = 1;
    protected int splitValue = 0;

    public Layer rootLayer;

    #endregion

    #region Property
    public DateTime SimTime
    {
        get
        {
            return simTime;
        }
        set
        {
            simTime = value;
            InitAllModel();
        }
    }

    #endregion

    [SerializeField]
    protected CanvasController canvasController;

    // Start is called before the first frame update
    protected void Start()
    {
        satelliteModels = new Dictionary<string, BaseSatelliteModel>();
        earth = FindObjectOfType<EarthModel>();
        simTime = DateTime.UtcNow;
        categories = new Dictionary<string, List<BaseSatelliteModel>>();
        rootLayer = new Layer(false);

        if (DivisionValue < 1)
        {
            DivisionValue = 1;
        }

        if (string.IsNullOrEmpty(categoriesPath))
        {
            List<string> str = new List<string>();
            //Test Case
            str.Add("SGP4 Test");
            str.Add("1 25544U 98067A   21091.70445102  .00001678  00000-0  38648-4 0  9991");
            str.Add("2 25544  51.6471   1.0083 0003014 167.9185 235.2837 15.48972976276747");
            Layer testLayer = new Layer(true);
            testLayer.addObject( "SGP4 Test", ProcessTLE(str.ToArray(), "Test"));
            rootLayer.addObject("test", testLayer);
        }
        else
        {
            try
            {
                StreamReader sr = new StreamReader(Application.streamingAssetsPath + Path.DirectorySeparatorChar + categoriesPath);
                while (!sr.EndOfStream)
                {
                    string path = Application.streamingAssetsPath + Path.DirectorySeparatorChar + sr.ReadLine();
                    ProcessDataFiles(rootLayer, path);
                }
                sr.Close();
            }
            catch (Exception e)
            {
                Debug.Log("Exception: " + e.Message);
            }
        }

        splitUpdate = new List<BaseSatelliteModel>[50];
        for(int i = 0; i < DivisionValue; i++)
        {
            splitUpdate[i] = new List<BaseSatelliteModel>();
        }

        int j = 0;
        foreach (BaseSatelliteModel satelliteModel in satelliteModels.Values)
        {
            splitUpdate[j % DivisionValue].Add(satelliteModel);
            j++;
        }

        InitAllModel();
        canvasController.Initialization(rootLayer, this);
    }

    public void FixedUpdate()
    {
        UpdateAllModel();

        splitValue++;
        splitValue %= DivisionValue;
    }

    public void UpdateAllModel()
    {
        simTime = simTime.AddSeconds(Time.deltaTime * timeScaling);
        earth.ReferenceTime = simTime;
        canvasController?.UpdateDate(simTime.ToString());
        foreach (BaseSatelliteModel satelliteModel in splitUpdate[splitValue])
        {
            satelliteModel.ReferenceTime = simTime;
        }
    }

    public void InitAllModel()
    {
        simTime = simTime.AddSeconds(Time.deltaTime * timeScaling);
        earth.ReferenceTime = simTime;

        foreach (BaseSatelliteModel satelliteModel in satelliteModels.Values)
        {
            satelliteModel.ReferenceTime = simTime;
        }
    }



    public void ProcessDataFiles(Layer currentLayer, string filePath)
    {
        try
        {
            StreamReader sr = new StreamReader(filePath);
            string[] path = filePath.Split(Path.DirectorySeparatorChar);
            string category = path[path.Length - 1].Split('.')[0];
            if(category.ToLower().Contains("category"))
            {
                Layer catLayer = new Layer(false);
                currentLayer.addObject(category.Split(',')[0], catLayer);
                while (!sr.EndOfStream)
                {
                    ProcessDataFiles(catLayer, sr.ReadLine());
                }
            }
            else
            {
                Layer satLayer = new Layer(true);
                currentLayer.addObject(category, satLayer);
                while (!sr.EndOfStream)
                {
                    List<string> str = new List<string>();

                    //Test Case
                    str.Add(sr.ReadLine());
                    str.Add(sr.ReadLine());
                    str.Add(sr.ReadLine());
                    BaseSatelliteModel sat =  ProcessTLE(str.ToArray(), category);
                    satLayer.addObject(sat.name, sat);
                }
                sr.Close();
            }
        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e.Message);
        }
    }


    public BaseSatelliteModel ProcessTLE(string[] tleInformation, string category)
    {
        Tle tle = new Tle(tleInformation[0], tleInformation[1], tleInformation[2]);
        if (satelliteModels.ContainsKey(tleInformation[0]))
        {
            if (category.ToLower().Contains("active"))
            {
                satelliteModels[tleInformation[0]].ActiveSatelliteModel = true;
            }

            if (!categories.ContainsKey(category))
            {
                categories.Add(category, new List<BaseSatelliteModel>());
            }
            categories[category].Add(satelliteModels[tleInformation[0]]);

            satelliteModels[tleInformation[0]].Categories.Add(category);
            return satelliteModels[tleInformation[0]];
        }
        else
        {
            GameObject tempSatellite = Instantiate(satellitePrefab, baseCoordinateSystem);
            BaseSatelliteModel model = tempSatellite.GetComponent<BaseSatelliteModel>();
            model.ActiveSatelliteModel = true;
            model.Initialize(tle, category);
            model.ReferenceTime = simTime;
            satelliteModels.Add(tle.Name, model);

            if (!categories.ContainsKey(category))
            {
                categories.Add(category, new List<BaseSatelliteModel>());
            }
            categories[category].Add(satelliteModels[tleInformation[0]]);
            return model;
        }
    }
}