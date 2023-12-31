using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public int maxPlayers;

    //instance
    public static NetworkManager instance;

    private void Awake()
    {
        if (instance != null && instance != this) {
            gameObject.SetActive(false);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        //connect to the master server
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        // Debug.Log("Connected to our master server!");
        PhotonNetwork.JoinLobby();
    }

    // public override void OnJoinedLobby()
    // {
    //     Debug.Log("Joined the lobby!");
    //     CreateRoom("testroom");
    // }

    public void CreateRoom(string roomName)
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)maxPlayers;

        PhotonNetwork.CreateRoom(roomName, options);
    }

    // public override void OnJoinedRoom()
    // {
    //     Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
    // }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}
