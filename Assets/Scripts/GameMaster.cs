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

    // �J�[�h�𐶐����Ĕz��
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
                // enemy����J�[�h���o��
                enemy.RandomSubmit();
            }
        }
        else if (enemy.IsSubmitted)
        {
            // Player�̒�o��҂�
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

    // Card�̏�������
    // ������ƒx�点�Ă��猋�ʂ�\���F�R���[�`��
    // �\�����I�������A���̃^�[���ɂ���(��̃J�[�h���̂Ă�)

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

    // �\�����I�������A���̃^�[���ɂ���(��̃J�[�h���̂Ă�)
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
            // TODO:Player����ɏo���i���ăp�l����\���j
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

    // Player���J�[�h���o�����Ƃ�
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

    // �Đ��]:retry����������
    void SendRetryMessage()
    {
        photonView.RPC(nameof(OnRecieveRetryMessage), RpcTarget.Others);
    }

    // �󂯎�����Ƃ��Ɏ��s
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
