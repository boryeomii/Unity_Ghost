using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scoreManager : MonoBehaviourPun, IPunObservable
{
    private static scoreManager m_instance;

    public int masterScore { get; private set; }
    public int clientScore { get; private set; }

    public Text masterscoreText;
    public Text clientscoreText;

    private int masterScoreMultiplier = 1;
    private int clientScoreMultiplier = 1;

    private double masterMultiplierEndTime;
    private double clientMultiplierEndTime;

    public static scoreManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindFirstObjectByType<scoreManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    } 
    private void Awake() 
    {
        if (instance != this) 
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        masterScore = 0;
        clientScore = 0;
    }
    
    public void UpdateMasterScoreText(int newScore)
    { 
        masterscoreText.text = "" + newScore;
    }

    public void UpdateClientScoreText(int newScore)
    {
        clientscoreText.text = "" + newScore;
    }

    public void AddMasterScore(int newScore) 
    {
        UpdateScoreMultiplier();

        masterScore += newScore * masterScoreMultiplier;
        UpdateMasterScoreText(masterScore);           
    }

    public void AddClientScore(int newScore)
    {
        UpdateScoreMultiplier();

        clientScore += newScore * clientScoreMultiplier;
        UpdateClientScoreText(clientScore);    
    }

    public void MasterPenaltyScore(int newScore)
    {
        if (!GameManager.instance.isGameover && masterScore > 0)
        {          
            masterScore -= newScore;
            UpdateMasterScoreText(masterScore);          
        }
    }

    public void ClientPenaltyScore(int newScore)
    {
        if (!GameManager.instance.isGameover&&clientScore > 0)
        {
            clientScore -= newScore;
            UpdateClientScoreText(clientScore);
        }
    }

    public void ActivateScoreMultiplier(bool isMasterPlayer)
    {
        if (isMasterPlayer)
        {
            masterScoreMultiplier = 2;
            masterMultiplierEndTime = PhotonNetwork.Time + 5.0;
        }
        else
        {
            clientScoreMultiplier = 2;
            clientMultiplierEndTime = PhotonNetwork.Time + 5.0;
        }
    }

    private void UpdateScoreMultiplier()
    {
        if (masterScoreMultiplier > 1 &&
            PhotonNetwork.Time >= masterMultiplierEndTime)
        {
            masterScoreMultiplier = 1;
        }

        if (clientScoreMultiplier > 1 &&
            PhotonNetwork.Time >= clientMultiplierEndTime)
        {
            clientScoreMultiplier = 1;
        }
    }

    public void StealForMasterScore()
    {
        if (!GameManager.instance.isGameover && clientScore >= 3)
        {
            masterScore += 3;
            clientScore -= 3;

            UpdateClientScoreText(clientScore);
            UpdateMasterScoreText(masterScore);
        }
    }

    public void StealForClientScore()
    {
        if (!GameManager.instance.isGameover && masterScore >= 3)
        {
            masterScore -= 3;
            clientScore += 3;

            UpdateClientScoreText(clientScore);
            UpdateMasterScoreText(masterScore);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {   
        if (stream.IsWriting)
        {
            stream.SendNext(masterScore);
            stream.SendNext(clientScore);
        }
        else
        {
            masterScore = (int)stream.ReceiveNext();
            clientScore = (int)stream.ReceiveNext();
            UpdateMasterScoreText(masterScore);
            UpdateClientScoreText(clientScore);
        }
    }

    public void ResetScore()
    {
        masterScore = 0;
        clientScore = 0;

        masterScoreMultiplier = 1;
        clientScoreMultiplier = 1;

        masterMultiplierEndTime = 0;
        clientMultiplierEndTime = 0;

        masterscoreText.text = "0";
        clientscoreText.text = "0";
    }
}
