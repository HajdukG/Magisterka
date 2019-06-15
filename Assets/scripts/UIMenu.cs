using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    public NetworkOperation network;
    public Slider slider;
    bool switcher = true;
    public void ShowHideMenu()
    {
        if(switcher)
        {
            slider.gameObject.SetActive(true);
            switcher = false;
        }
        else
        {
            slider.gameObject.SetActive(false);
            switcher = true;
        }
        Debug.Log("ShowHideMenu");
    }

    public void StopThread()
    {
        network.StopThread();
    }
}
