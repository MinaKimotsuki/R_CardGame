using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

// MonoBehaviourPunCallbacks���p�����āAPUN�̃R�[���o�b�N���󂯎���悤�ɂ���
public class OnlineMenuManager : MonoBehaviourPunCallbacks
{
    // �{�^������������}�b�`���O�J�n
    // �����_���}�b�`���O�ŒN���̕����ɓ���΃}�b�`���O����
    // �������Ȃ���Ύ����ō��
    // ������2���ɂȂ�΃V�[����J��

    bool inRoom;
    bool isMatching;

    public void OnMatchingButton()
    {
        // PhotonServerSettings�̐ݒ���e���g���ă}�X�^�[�T�[�o�[�֐ڑ�����
        PhotonNetwork.ConnectUsingSettings();
    }

    // �}�X�^�[�T�[�o�[�ւ̐ڑ��������������ɌĂ΂��R�[���o�b�N
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    // �Q�[���T�[�o�[�ւ̐ڑ��������������ɌĂ΂��R�[���o�b�N
    public override void OnJoinedRoom()
    {
        inRoom = true;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 2 }, TypedLobby.Default);
    }

    // ������2�l�Ȃ�V�[�����A��
    private void Update()
    {
        if (isMatching)
        {
            return;
        }
        if (inRoom)
        {
            if (PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                isMatching = true;
                SceneManager.LoadScene("Game");
            }
        }
    }
}
