using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stealPack : MonoBehaviourPun, IItem
{
    public void Apply(GameObject target)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        playerController player = target.GetComponent<playerController>();

        if (player.photonView.Owner.IsMasterClient)
        {
            scoreManager.instance.StealForMasterScore();
        }
        else
        {
            scoreManager.instance.StealForClientScore();
        }

        PhotonNetwork.Destroy(gameObject);
    }
}
