using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraChangeManager : MonoBehaviour
{
    public List<CinemachineVirtualCamera> cameras = new();
    public List<NewFootIk> debug = new();
    public int currentCameraIndex = 0;
    public Button nextButton;
    public Button prevButton;

    private void Start()
    {
        currentCameraIndex = 0;
        ChangeCamera(0);
    }
    public void ChangeCamera(int index)
    {
        for (int i = 0; i < cameras.Count; i++)
        {
            if (i == index)
            {
                cameras[i].enabled = true;
            }
            else
            {
                cameras[i].enabled = false;
            }
        }

        if (currentCameraIndex - 1 < 0)
        {
            prevButton.interactable = false;
        }
        else
        {
            prevButton.interactable = true;
        }

        if (currentCameraIndex == cameras.Count - 1)
        {
            nextButton.interactable = false;
        }
        else
        {
            nextButton.interactable = true;  
        }
    }

    public void NextCamera()
    {
        if (currentCameraIndex + 1 < cameras.Count)
        {
            currentCameraIndex++;
            ChangeCamera(currentCameraIndex);
        }
    }

    public void PreviousCamera()
    {
        if (currentCameraIndex - 1 >= 0)
        {
            currentCameraIndex--;
            ChangeCamera(currentCameraIndex);
        }
    }

    public void ToggleDebug()
    {
        foreach (var footIK in debug)
        {
            footIK.debugShow = !footIK.debugShow;
        }
    }
}
