using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public TMP_InputField roomName_input; // �� �̸� ����
    public Transform content;
    public GameObject roomListingPrefab; // �� ����Ʈ prefab
    public static string roomName;  // �� �̸�

    void Start()
    {
        // ���� ���� (��: �ƽþ� ����)
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "asia";

        // ������ ����
        PhotonNetwork.ConnectUsingSettings();

        // �ڵ� �� ����ȭ Ȱ��ȭ
        PhotonNetwork.AutomaticallySyncScene = true;

        // Play BGM
        AudioManager.instance.PlayBgm(true);

    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("������ ���� ����");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("�κ� ����");
    }

    public void SetRoom()
    {  // ��ư Ŭ�� �� �� ����
        // Play SFX
        AudioManager.instance.PlaySfx(AudioManager.Sfx.StartBtn);

        roomName = roomName_input.text;
        if (!string.IsNullOrEmpty(roomName))
        {
            CreateRoomListing(roomName, 0);
            PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 2, IsVisible = true, IsOpen = true });
        }
    }

    private void CreateRoomListing(string roomName, int playerCount)
    {   // �� ��� ����
        GameObject roomListing = Instantiate(roomListingPrefab, content);
        TextMeshProUGUI roomText = roomListing.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        if (roomText != null)
        {
            roomText.text = $"{roomName} ({playerCount}/2)";
        }
        else
        {
            Debug.Log("error!");
        }

        Button roomButton = roomListing.GetComponent<Button>();
        if (roomButton != null)
        {
            roomButton.onClick.AddListener(() => OnClickJoinRoom(roomName));
        }
        else
        {
            Debug.Log("no btn!");
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("�� ���� ����: " + message);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("�� �̸�: " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("�� ��� ������Ʈ��. �� ��: " + roomList.Count);

        foreach (Transform child in content)
        {
            Destroy(child.gameObject); // ���� �� ��� ����
        }

        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
            {
                continue; // ���ŵ� ���� ����Ʈ�� ǥ������ ����
            }

            CreateRoomListing(room.Name, room.PlayerCount);
        }
    }

    public void OnClickJoinRoom(string roomName)
    {   // �� ������ ����
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = 2, IsVisible = true, IsOpen = true }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MatchLoading"); // �� ��ȯ
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Join room failed: " + message);
    }
}
