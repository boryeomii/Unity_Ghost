using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//점수 2배 되는 아이템
public class multiPack : MonoBehaviourPun, IItem
{   
    public void Apply(GameObject target)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        playerController player = target.GetComponent<playerController>();

        bool isMasterPlayer = player.photonView.Owner.IsMasterClient;

        scoreManager.instance.ActivateScoreMultiplier(isMasterPlayer);

        PhotonNetwork.Destroy(gameObject);
    }
}
