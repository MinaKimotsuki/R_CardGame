using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPanelController : MonoBehaviour
{
    [SerializeField] GameObject mainPanel;

    private void Start()
    {
        mainPanel.SetActive(false);
    }

    public void OnRuleButton()
    {
        mainPanel.SetActive(true);
    }

    public void OnCloseButton()
    {
        mainPanel.SetActive(false);
    }
}
