using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bone : MonoBehaviourPun, IItem
{
    private const int baseScore = 1;
    public void Apply(GameObject target)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        playerController player = target.GetComponent<playerController>();

        if (player.photonView.Owner.IsMasterClient)
        {
            scoreManager.instance.AddMasterScore(baseScore);
        }
        else
        {
            scoreManager.instance.AddClientScore(baseScore);
        }

        PhotonNetwork.Destroy(gameObject);
    }
}
