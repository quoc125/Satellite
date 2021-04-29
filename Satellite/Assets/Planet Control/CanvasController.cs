using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class CanvasController : MonoBehaviour
{
    [SerializeField]
    protected TextMeshProUGUI dateTime;
    [SerializeField]
    protected TMP_Dropdown categories;

    [SerializeField]
    protected TMP_Dropdown subCategories;

    [SerializeField]
    protected TMP_Dropdown subSubCategories;

    protected TMP_Dropdown currentDropbox;

    protected bool activeMenu = false;
    [SerializeField]
    protected PlayerInput inputSystem;

    [SerializeField]
    protected EventSystem eventHandler;

    protected Layer rootLayer;
    protected Layer subLayer;
    protected Layer subSubLayer;

    protected SatelliteManager satelliteManager;

    protected Vector3 orginalPosition;

    protected float orginalTimeScaling;

    public void UpdateDate(string time)
    {
        dateTime.text = time;
    }

    public void Initialization(Layer root, SatelliteManager manager)
    {
        rootLayer = root;
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        List<string> keys = rootLayer.getKeys();
        options.Add(new TMP_Dropdown.OptionData(""));

        foreach(string name  in keys)
        {
            options.Add(new TMP_Dropdown.OptionData(name));
        }

        categories.options = options;

        categories.onValueChanged.AddListener(delegate { ProcessCategoriesSelections(categories); });

        satelliteManager = manager;

        categories.gameObject.SetActive(false);
        subCategories.gameObject.SetActive(false);
        subSubCategories.gameObject.SetActive(false);
    }


    public void OpenMenu(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            activeMenu = !activeMenu;
            categories.gameObject.SetActive(activeMenu);
             

            if (activeMenu)
            {
                inputSystem?.SwitchCurrentActionMap("UI");
                eventHandler.SetSelectedGameObject(categories.gameObject);
                orginalTimeScaling = satelliteManager.timeScaling;
                satelliteManager.timeScaling = 0;

                categories.value = 0;
            }
            else
            {
                inputSystem?.SwitchCurrentActionMap("Player");
                eventHandler.SetSelectedGameObject(null);
                satelliteManager.timeScaling = orginalTimeScaling;
            }
        }
    }

    public void ProcessCategoriesSelections(TMP_Dropdown dropdown)
    {
        if (dropdown.value != 0)
        {
            subLayer = (Layer)rootLayer.getObject(dropdown.options[dropdown.value].text);

            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            List<string> keys = subLayer.getKeys();
            options.Add(new TMP_Dropdown.OptionData(""));

            foreach (string name in keys)
            {
                options.Add(new TMP_Dropdown.OptionData(name));
            }

            subCategories.options = options;

            subCategories.onValueChanged.AddListener(delegate { ProcessSubCategoriesSelections(subCategories); });

            subCategories.gameObject.SetActive(true);
            subLayer.setOutline(true);

        }
        else
        {
            subCategories.value = 0;
            subCategories.gameObject.SetActive(false);
            rootLayer.setOutline(false);
        }
    }

    public void ProcessSubCategoriesSelections(TMP_Dropdown dropdown)
    {
        if (dropdown.value != 0)
        {
            if (subLayer.storeSatellite)
            {
                BaseSatelliteModel sat = (BaseSatelliteModel)subLayer.getObject(dropdown.options[dropdown.value].text);
                subLayer.setOutline(false);
                orginalPosition = Camera.main.transform.position;
                Camera.main.transform.position = sat.transform.position;
                currentDropbox = subCategories;
                inputSystem?.SwitchCurrentActionMap("Look Around");

            }
            else
            {
                subSubLayer = (Layer)subLayer.getObject(dropdown.options[dropdown.value].text);

                List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
                List<string> keys = subSubLayer.getKeys();
                options.Add(new TMP_Dropdown.OptionData(""));

                foreach (string name in keys)
                {
                    options.Add(new TMP_Dropdown.OptionData(name));
                }

                subSubCategories.options = options;

                subSubCategories.onValueChanged.AddListener(delegate { ProcessSubSubCategoriesSelections(categories); });

                subSubCategories.gameObject.SetActive(true);
                subSubLayer.setOutline(true);

            }
        }
        else
        {
            subCategories.value = 0;
            subCategories.gameObject.SetActive(false);
            subLayer.setOutline(false);
        }
    }

    public void ProcessSubSubCategoriesSelections(TMP_Dropdown dropdown)
    {
        if (dropdown.value != 0)
        {
            if (subLayer.storeSatellite)
            {
                BaseSatelliteModel sat = (BaseSatelliteModel)subSubLayer.getObject(dropdown.options[dropdown.value].text);
                subSubLayer.setOutline(false);
                orginalPosition = Camera.main.transform.position;
                Camera.main.transform.position = sat.transform.position;
                currentDropbox = subSubCategories;
                inputSystem?.SwitchCurrentActionMap("Look Around");

            }
        }
    }

    public void cancelLookAt (InputAction.CallbackContext context)
    {
        Camera.main.transform.position = orginalPosition;
        currentDropbox.value = 0;
        inputSystem?.SwitchCurrentActionMap("UI");
    }

}
