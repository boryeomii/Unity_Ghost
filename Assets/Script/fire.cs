using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class fire : MonoBehaviourPun, IItem
{
    private Rigidbody2D fireRigidbody;

    [SerializeField]
    private float speed = 4f;

    [SerializeField]
    private int scorePenalty = 1;

    private void Awake()
    {
        fireRigidbody = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        fireRigidbody.linearVelocity = direction * speed;
    }

    public void Apply(GameObject target)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        playerController player = target.GetComponent<playerController>();

        if (player.photonView.Owner.IsMasterClient)
        {
            scoreManager.instance.MasterPenaltyScore(scorePenalty);
        }
        else
        {
            scoreManager.instance.ClientPenaltyScore(scorePenalty);
        }

        PhotonNetwork.Destroy(gameObject);

    }   
}
