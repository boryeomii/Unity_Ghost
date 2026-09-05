using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemDestroy : MonoBehaviourPun,IItem
{
    public void Apply(GameObject target)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        PhotonNetwork.Destroy(gameObject);
    }
}