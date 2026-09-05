using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

// 마스터(매치 메이킹) 서버와 룸 접속을 담당
public class LobbyManager : MonoBehaviourPunCallbacks 
{
    private string gameVersion = "1"; // 게임 버전

    public Text connectionInfoText; // 네트워크 정보를 표시할 텍스트
    public Button joinButton; // 룸 접속 버튼

    // 게임 실행과 동시에 마스터 서버 접속 시도
    private void Start() 
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.GameVersion = gameVersion;

        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        joinButton.interactable = false;
        connectionInfoText.text = "Connecting to master server...";
    }

    // 마스터 서버 접속 성공시 자동 실행
    public override void OnConnectedToMaster() 
    {
        Debug.Log(
        $"[Photon] Connected | " +
        $"Region={PhotonNetwork.CloudRegion} | " +
        $"GameVersion={PhotonNetwork.GameVersion}"
        );

        joinButton.interactable = true;
        connectionInfoText.text = "Online: connected master server";
    }

    // 마스터 서버 접속 실패시 자동 실행
    public override void OnDisconnected(DisconnectCause cause) 
    {
        joinButton.interactable = false;
        connectionInfoText.text = "Offline: No connection with the master server.\n Retrying connection...";
        PhotonNetwork.ConnectUsingSettings();
    }

    // 룸 접속 시도
    public void Connect() 
    {
        joinButton.interactable = false;
        if (PhotonNetwork.IsConnected)
        {
            connectionInfoText.text = "Access the room";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            connectionInfoText.text = "Offline: No connection with the master server.\n Retrying connection...";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // (빈 방이 없어)랜덤 룸 참가에 실패한 경우 자동 실행
    public override void OnJoinRandomFailed(short returnCode, string message) {
        connectionInfoText.text = "No empty room, create new room...";
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }

    // 룸에 참가 완료된 경우 자동 실행
    public override void OnJoinedRoom() 
    {
        Debug.Log(
        $"[Photon] Joined Room | " +
        $"Room={PhotonNetwork.CurrentRoom.Name} | " +
        $"Players={PhotonNetwork.CurrentRoom.PlayerCount} | " +
        $"Master={PhotonNetwork.IsMasterClient} | " +
        $"Region={PhotonNetwork.CloudRegion}"
        );

        connectionInfoText.text = "Room join success!";
        if (PhotonNetwork.IsMasterClient) PhotonNetwork.LoadLevel("Gamemap");
    }
}