using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameUI : MonoBehaviour
{
    [SerializeField] Text turnResultText;
    [SerializeField] Text playerLifeText;
    [SerializeField] Text enemyLifeText;
    [SerializeField] GameObject resultPanel;
    [SerializeField] GameObject resultPanelRetryButton;
    [SerializeField] Text resultText;
    [SerializeField] GameObject playerAddNumberObj;
    [SerializeField] GameObject enemyAddNumberObj;
    [SerializeField] GameObject rulePanel;
    [SerializeField] GameObject leavePanel;
    [SerializeField] GameObject retryMessage;
    [SerializeField] GameObject retryButton;

    public void Init()
    {
        turnResultText.gameObject.SetActive(false);
        resultPanel.SetActive(false);
        leavePanel.SetActive(false);
        retryMessage.SetActive(false);
        resultPanelRetryButton.SetActive(true);
        if (GameDataManager.Instance.IsOnlineBattle)
        {
            retryButton.SetActive(false);
        }
    }

    public void UpdateAddNumber(int playerAddNumber, int enemyAddNumber)
    {
        if (playerAddNumber == 2)
        {
            playerAddNumberObj.SetActive(true);
        }
        else
        {
            playerAddNumberObj.SetActive(false);
        }

        if (enemyAddNumber == 2)
        {
            enemyAddNumberObj.SetActive(true);
        }
        else
        {
            enemyAddNumberObj.SetActive(false);
        }
    }

    public void ShowLifes(int playerLife, int enemyLife)
    {
        playerLifeText.text = $"x{playerLife}";
        enemyLifeText.text = $"x{enemyLife}";
    }

    // É^Å[ÉìÇÃèüîsï\é¶
    public void ShowTurnResult(string result)
    {
        turnResultText.gameObject.SetActive(true);
        turnResultText.text = result;
    }

    public void ShowGameResult(string result)
    {
        resultPanel.SetActive(true);
        resultText.text = result;
    }

    public void SetupNextTurn()
    {
        turnResultText.gameObject.SetActive(false);
    }

    public void OnRuleButton()
    {
        rulePanel.SetActive(true);
        rulePanel.transform.localScale = Vector3.zero;
        rulePanel.transform.DOScale(1, 0.1f);
    }

    public void OnCloseRuleButton()
    {
        rulePanel.transform.DOScale(0, 0.1f);
    }

    public void ShowLeavePanel()
    {
        leavePanel.SetActive(true);
    }

    public void ShowRetryMessage()
    {
        retryMessage.SetActive(true);
    }

    public void HideRetryButton()
    {
        resultPanelRetryButton.SetActive(false);
    }
}
