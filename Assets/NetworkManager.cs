using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Others")]
    public GameObject Others;

    [Header("DisconnectPanel")]
    public InputField Nickname;
    public GameObject DisconnectPanel;

    [Header("LobbyPanel")]
    public Text LobbyInfo;
    public GameObject LobbyPanel;
    public InputField RoomJoinTitle;
    public Button[] CellBtn;
    public Button PrevBtn;
    public Button NextBtn;
    public Button CreateBtn;
    public Button BackBtn;
    public Button JoinBtn;

    [Header("RoomPanel")]
    public GameObject RoomPanel;
    public Text RoomInfoTitle;
    public Text RoomInfoParti;
    public GameObject[] Player_list;
    public Button ExitBtn;
    public Button SendBtn;
    public Text ChatInput;
    public Text[] ChatText;

    [Header("ETC")]
    public Text StatusText;
    public PhotonView PV;
    public GameObject Grid;


    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 0, minPage = 0, maxPage = 0;
    int my_turn_inRoom = 0;

    private void Awake()
    {
        // component ���� networkmanger inspection���� �ϳ��ϳ� ��ġ���ش�.
        Screen.SetResolution(960, 540, false); 
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }
    // Update is called once per frame
    void Update()
    {
        StatusText.text = PhotonNetwork.NetworkClientState.ToString();
        LobbyInfo.text = "�κ� "+ (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "��\n���� " + PhotonNetwork.CountOfPlayers + "��";
        // ���� Ű �Ǵ� Ű�е��� ���� Ű�� ������ ��
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SendBtn.onClick.Invoke();
        }
    }

    public void Connection()
    {
        if (Nickname.text.Length > 0)
        {
            DisconnectPanel.SetActive(false);
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Connecting to Photon Server...");
        }
        else
        {

        }
    }

    // ���� ������ �Ϸ�Ǹ� �ݹ�Ǵ� �Լ�
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        LobbyPanel.SetActive(true);
        RoomPanel.SetActive(false);
        PhotonNetwork.LocalPlayer.NickName = Nickname.text;
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);
        myList.Clear();
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(false);
    }

    // �� ����
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom("Room" + Random.Range(0, 100), new RoomOptions { MaxPlayers = 4 });
    }

    // �� ���� ���� �ݹ�
    public override void OnCreateRoomFailed(short returnCode, string message) {
        CreateRoom(); 
    }

    // ���� �����ǰų� ���ŵ� ��, �ݹ�
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                else myList[myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        RoomListRefresh();
    }

    // �� ���� ����, �κ� ���� �� 
    void RoomListRefresh() {

        int _size = myList.Count;
        maxPage = (_size + 3) / 4;

        Debug.Log(_size);

        PrevBtn.interactable = currentPage > 0 ? true : false;
        NextBtn.interactable = currentPage == maxPage ? false : true;
        

        for(int i = 0; i < 4; i++)
        {
            if (i < _size)
            {
                CellBtn[i].transform.GetChild(0).GetComponent<Text>().text = myList[i].Name;
                CellBtn[i].transform.GetChild(1).GetComponent<Text>().text = myList[i].PlayerCount + "/" + myList[i].MaxPlayers;
                CellBtn[i].interactable = true;
            }
            else
            {
                CellBtn[i].transform.GetChild(0).GetComponent<Text>().text = "";
                CellBtn[i].transform.GetChild(1).GetComponent<Text>().text = "";
                CellBtn[i].interactable = false;
            }
        }
    }

    public void Player_LeaveRoom() 
    {
        PhotonNetwork.LeaveRoom();
        LobbyPanel.SetActive(true);
        RoomPanel.SetActive(false);
        RoomListRefresh();
        RoomRenewal();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomRenewal();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomRenewal();
    }

    public void RoomRenewal()
    {
        int _size = PhotonNetwork.PlayerList.Length;
        for (int i = 0; i < _size; i++)
        {
            Player_list[i].SetActive(true);
            Player_list[i].transform.Find("ID").GetComponent<Text>().text = PhotonNetwork.PlayerList[i].NickName;
            Button btn = null;
            if (i == 0)
            {
                btn = Player_list[i].transform.Find("StartBtn").GetComponent<Button>();
            }
            else
            {
                btn = Player_list[i].transform.Find("ReadyBtn").GetComponent<Button>();
            }
            if (PhotonNetwork.PlayerList[i].NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                my_turn_inRoom = i;
                btn.gameObject.SetActive(true);
            }
            else
            {
                btn.gameObject.SetActive(false);
            }
        }
        for(int i = _size; i <4; i++)
        {
            Player_list[i].SetActive(false);
        }

        RoomInfoParti.text = _size + " / 4";
    }

    // �濡 ������ �� 
    public override void OnJoinedRoom()
    {
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(true);
        RoomInfoTitle.text = PhotonNetwork.CurrentRoom.Name;
        RoomRenewal();
        
        //ChatInput.text = "";
        //for (int i = 0; i < ChatText.Length; i++) ChatText[i].text = "";
    }

    public void JoinRoomSelect(Button clickedBtn)
    {
        string roomtitle = clickedBtn.transform.Find("RoomTitle").GetComponent<Text>().text;
        PhotonNetwork.JoinRoom(roomtitle);
        RoomRenewal();
    }

    // ������ GameStart ��ư�� ������ ��

    public void OnStartButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("StartGame", RpcTarget.All);
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
    }


    public void Send()
    {
        PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text);
        ChatInput.text = "";
    }

    [PunRPC] // RPC�� �÷��̾ �����ִ� �� ��� �ο����� �����Ѵ�

    void ChatRPC(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++)
            if (ChatText[i].text == "")
            {
                isInput = true;
                ChatText[i].text = msg;
                break;
            }
        if (!isInput) // ������ ��ĭ�� ���� �ø�
        {
            for (int i = 1; i < ChatText.Length; i++) ChatText[i - 1].text = ChatText[i].text;
            ChatText[ChatText.Length - 1].text = msg;
        }
    }

    [PunRPC]
    public void StartGame()
    {
        // DisconnectPanel.SetActive(false);
        // LobbyPanel.SetActive(false);
        Others.SetActive(false);
        RoomPanel.SetActive(false);
        Grid.SetActive(true);

        Vector3 spawnPosition = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        PhotonNetwork.Instantiate("Player", spawnPosition, Quaternion.identity);
    }

}
