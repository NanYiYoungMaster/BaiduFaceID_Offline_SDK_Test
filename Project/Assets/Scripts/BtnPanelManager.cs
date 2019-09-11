using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnPanelManager : MonoBehaviour
{
    public GameObject MainPanel;
    public GameObject BackButton;
    public Face faceCheckController;

    public void BeginLoginOnClick()
    {
        BackButton.SetActive(false);
        MainPanel.SetActive(true);
        faceCheckController.BeginLogin();
    }

    public void BeginRegisterOnClick()
    {
        BackButton.SetActive(false);
        MainPanel.SetActive(true);
        faceCheckController.BeginRegister();
    }

    public void ShowBackButton()
    {
        BackButton.SetActive(true);
    }

    public void BackToBtnPanelOnClick()
    {
        MainPanel.SetActive(false);
    }
}
