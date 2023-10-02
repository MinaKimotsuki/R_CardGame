using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameMaster : MonoBehaviourPunCallbacks
{
    [SerializeField] Battler player;
    [SerializeField] Battler enemy;
    [SerializeField] CardGenerator cardGenerator;
    [SerializeField] GameUI gameUI;
    RuleBook ruleBook;
    bool playerRetryReady;
    bool enemyRetryReady;

    private void Awake()
    {
        ruleBook = GetComponent<RuleBook>();
    }
    private void Start()
    {
        Setup();
    }

    // カードを生成して配る
    void Setup()
    {
        playerRetryReady = false;
        enemyRetryReady = false;
        gameUI.Init();
        player.Life = 4; 
        enemy.Life = 4;
        gameUI.ShowLifes(player.Life, enemy.Life);
        gameUI.UpdateAddNumber(player.AddNumber, enemy.AddNumber);
        player.OnSubmitAction = SubmittedAction;
        enemy.OnSubmitAction = SubmittedAction;
        SendCardsTo(player, isEnemy: false);
        SendCardsTo(enemy, isEnemy: true);

        if (GameDataManager.Instance.IsOnlineBattle)
        {
            player.OnSubmitAction += SendPlayerCard;
        }
    }

    void SubmittedAction()
    {
        if (player.IsSubmitted && enemy.IsSubmitted)
        {
            StartCoroutine(CardsBattle());
        }
        else if (player.IsSubmitted)
        {
            if (GameDataManager.Instance.IsOnlineBattle == false)
            {
                // enemyからカードを出す
                enemy.RandomSubmit();
            }
        }
        else if (enemy.IsSubmitted)
        {
            // Playerの提出を待つ
        }
    }

    void SendCardsTo(Battler battler, bool isEnemy)
    {
        for (int i = 0; i < 8; i++)
        {
            Card card = cardGenerator.Spawn(i, isEnemy);
            battler.SetCardToHand(card);
        }
        battler.Hand.ResetPosition();
    }

    // Cardの勝利判定
    // ちょっと遅らせてから結果を表示：コルーチン
    // 表示が終わったら、次のターンにうつる(場のカードを捨てる)

    IEnumerator CardsBattle()
    {
        yield return new WaitForSeconds(1f);
        enemy.SubmitCard.Open();
        yield return new WaitForSeconds(0.7f);
        Result result = ruleBook.GetResult(player, enemy);
        switch (result)
        {
            case Result.TurnWin:
            case Result.GameWin:
                gameUI.ShowTurnResult("WIN");
                enemy.Life--;
                break;
            case Result.TurnWin2:
                gameUI.ShowTurnResult("WIN");
                enemy.Life -= 2;
                break;
            case Result.TurnLose:
            case Result.GameLose:
                gameUI.ShowTurnResult("LOSE");
                player.Life--;
                break;
            case Result.TurnLose2:
                gameUI.ShowTurnResult("LOSE");
                player.Life -= 2;
                break;
            case Result.TurnDraw:
                gameUI.ShowTurnResult("DRAW");
                break;
        }
        gameUI.ShowLifes(player.Life, enemy.Life);
        yield return new WaitForSeconds(1f);

        if ((player.Hand.IsEmpty) || (result == Result.GameWin) || (result == Result.GameLose) || (player.Life <= 0 || enemy.Life <= 0))
        {
            ShowResult(result);
        }
        else
        {
            SetupNextTurn();
        }

    }

    void ShowResult(Result result)
    {
        if (result == Result.GameWin)
        {
            gameUI.ShowGameResult("WIN");
        }
        else if (result == Result.GameLose)
        {
            gameUI.ShowGameResult("LOSE");
        }
        else if (player.Life <= 0 && enemy.Life <= 0)
        {
            gameUI.ShowGameResult("DRAW");
        }
        else if (player.Life <= 0)
        {
            gameUI.ShowGameResult("LOSE");
        }
        else if (enemy.Life <= 0)
        {
            gameUI.ShowGameResult("WIN");
        }
        else if (player.Life > enemy.Life)
        {
            gameUI.ShowGameResult("WIN");
        }
        else if (player.Life < enemy.Life)
        {
            gameUI.ShowGameResult("LOSE");
        }
        else
        {
            gameUI.ShowGameResult("DRAW");
        }
    }

    // 表示が終わったら、次のターンにうつる(場のカードを捨てる)
    void SetupNextTurn()
    {
        player.SetupNextTurn();
        enemy.SetupNextTurn();
        gameUI.SetupNextTurn();
        gameUI.UpdateAddNumber(player.AddNumber, enemy.AddNumber);

        if (enemy.IsFirstSubmit)
        {
            enemy.IsFirstSubmit = false;
            if (GameDataManager.Instance.IsOnlineBattle == false)
            {
                enemy.RandomSubmit();
                enemy.SubmitCard.Open();
            }
        }
        if (player.IsFirstSubmit)
        {
            // TODO:Playerが先に出す（ってパネルを表示）
        }
    }

    public void OnRetryButton()
    {
        playerRetryReady = true;
        if (GameDataManager.Instance.IsOnlineBattle)
        {
            gameUI.HideRetryButton();
            SendRetryMessage();
            if (playerRetryReady && enemyRetryReady)
            {
                string currentScene = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(currentScene);
            }
        }
        else
        {
            string currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentScene);
        }
    }
    public void OnTitleButton()
    {
        if (GameDataManager.Instance.IsOnlineBattle)
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.Disconnect();
            }
        }
        SceneManager.LoadScene("Title");
    }

    // Playerがカードを出したとき
    void SendPlayerCard()
    {
        photonView.RPC(nameof(RPCOnRecievedCard), RpcTarget.Others, player.SubmitCard.Base.Number);
    }

    [PunRPC]
    void RPCOnRecievedCard(int number)
    {
        enemy.SetSubmitCard(number);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (GameDataManager.Instance.IsOnlineBattle)
        {
            gameUI.ShowLeavePanel();
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.Disconnect();
            }
        }
    }

    // 再戦希望:retryを押した方
    void SendRetryMessage()
    {
        photonView.RPC(nameof(OnRecieveRetryMessage), RpcTarget.Others);
    }

    // 受け取ったときに実行
    [PunRPC]
    void OnRecieveRetryMessage()
    {
        enemyRetryReady = true;
        if (playerRetryReady)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentScene);
        }
        gameUI.ShowRetryMessage();
    }
}
