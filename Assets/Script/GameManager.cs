using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks 
{ 
    private static GameManager m_instance;
    public static GameManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindFirstObjectByType<GameManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }
    public Button readyButton;
    public bool isReady { get; private set; }
    public bool isGameover { get; private set; }
    public GameObject playerPrefab;

    public Text timeText;

	[SerializeField]
	private double gameDuration = 120.0;
	private double gameStartTime;

	public double RemainingTime { get; private set; }

    public Button startButton;
    public GameObject gamestartUI;
    public Button restartButton;
    public GameObject gameoverUI;

	[SerializeField] private Text winText;
	[SerializeField] private Text loseText;
	[SerializeField] private Text drawText;

	private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            Destroy(gameObject);
        }

        Vector2 randomPos = Random.insideUnitSphere * 5f;
        randomPos.y = 0;
        PhotonNetwork.Instantiate(playerPrefab.name, randomPos, Quaternion.identity);

        isGameover = true;
        isReady = false;

        gamestartUI.SetActive(true);

        startButton.interactable = false;
        readyButton.interactable = false;
        restartButton.interactable = false;

        gameoverUI.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            readyButton.interactable = true;
        }

        if (PhotonNetwork.IsMasterClient && isReady)
        {
            startButton.interactable = true;
        }

        if (!isGameover)
        {
			UpdateTimer();
		}

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    [PunRPC]
    private void GameReadyRpc()
    {
        isReady = true;
    }
    public void GameReady()
    {
        photonView.RPC(nameof(GameReadyRpc), RpcTarget.MasterClient);
    }

    [PunRPC]
    private void StartGameRpc(double startTime)
    {
		gameStartTime = startTime;
		RemainingTime = gameDuration;

		isGameover = false;
        isReady = false;

        gamestartUI.SetActive(false);
        gameoverUI.SetActive(false);

        scoreManager.instance.ResetScore();
    }

    public void StartGame()
    {
		if (!PhotonNetwork.IsMasterClient) return;

        // 게임 시작 후 새로운 플레이어 입장 금지
        PhotonNetwork.CurrentRoom.IsOpen = false;

        double startTime = PhotonNetwork.Time;

		photonView.RPC(nameof(StartGameRpc), RpcTarget.All, startTime);
	}

    public void RestartGame()
    {
        StartGame();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (isGameover)
        {
            // 아직 게임 시작 전
            isReady = false;

            if (PhotonNetwork.IsMasterClient)
            {
                startButton.interactable = false;
            }

            return;
        }

        // 게임 진행 중 상대방이 나간 경우
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void EndGame()
    {
		if (!PhotonNetwork.IsMasterClient) return;

		photonView.RPC(nameof(EndGameRpc),RpcTarget.All);
    }

	[PunRPC]
	private void EndGameRpc()
	{
		isGameover = true;
		gameoverUI.SetActive(true);

		ShowGameResult();

		restartButton.interactable = PhotonNetwork.IsMasterClient;
	}

    private void ShowGameResult()
    {
		int masterScore = scoreManager.instance.masterScore;

		int clientScore = scoreManager.instance.clientScore;

		winText.enabled = false;
		loseText.enabled = false;
		drawText.enabled = false;

		if (masterScore == clientScore)
		{
			drawText.enabled = true;
			return;
		}

		bool masterWon = masterScore > clientScore;

		bool localPlayerWon = PhotonNetwork.IsMasterClient ? masterWon :!masterWon;

		winText.enabled = localPlayerWon;
		loseText.enabled = !localPlayerWon;
	}

	public void UpdateTimer()
    {
		double elapsedTime = PhotonNetwork.Time - gameStartTime;

		RemainingTime = gameDuration - elapsedTime;

		if (RemainingTime <= 0)
		{
			RemainingTime = 0;

			timeText.text = "0 : 00";

			if (PhotonNetwork.IsMasterClient)
			{
				EndGame();
			}

			return;
		}

		int minutes = (int)(RemainingTime / 60);
		int seconds = (int)(RemainingTime % 60);

		timeText.text = $"{minutes} : {seconds:00}";

	}
}
