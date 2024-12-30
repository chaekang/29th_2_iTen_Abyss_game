using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Making : MonoBehaviour
{
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // ���� �� ������ �����ϸ� �� ���� ����
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
        Debug.Log("create a new Room");
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError("Disconnected: " + cause);
        PhotonNetwork.ConnectUsingSettings(); // ���� ���� �� ������ �õ�
    }
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
            PhotonNetwork.AutomaticallySyncScene = true;
            //MakeTeams();
        }
        else
        {
            Debug.Log("Not in room..");
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            //PhotonNetwork.LoadLevel("Driving"); //���� ���� �ڵ� ����ȭ ��.    
            MakeTeams();
            Invoke("LoadDriving", 2.0f);
        }
    }

    private void LoadDriving()
    {
        PhotonNetwork.LoadLevel("Driving");
    }

    private void MakeTeams()
    {   // �� ������
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            Player[] players = PhotonNetwork.PlayerList;
            ExitGames.Client.Photon.Hashtable teamProperties = new ExitGames.Client.Photon.Hashtable();

            for (int i = 0; i < players.Length; i++)
            {
                if (i % 2 == 0)
                { // ¦�� ��° �Ʊ�
                    teamProperties["Team"] = "Our";
                }
                else
                {   // Ȧ�� ��° ����
                    teamProperties["Team"] = "Enemy";
                }
                players[i].SetCustomProperties(teamProperties);
                Debug.Log($"�÷��̾� {players[i].NickName} �� ����: {teamProperties["Team"]}");
            }
        }
    }
}