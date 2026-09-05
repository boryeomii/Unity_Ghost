using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class playerController : MonoBehaviourPun
{
    [SerializeField]
    private GameObject playerPointerUI;

    [SerializeField]
    private Image masterPointer;

    [SerializeField]
    private Image clientPointer;

    private PlayerItemEffects effects;

    private void Awake()
    {
        effects = GetComponent<PlayerItemEffects>();
    }

    private void Start()
    {
        playerPointerUI.SetActive(false);

        if (!photonView.IsMine) return;

        playerPointerUI.SetActive(true);

        bool isMaster =
            PhotonNetwork.IsMasterClient;

        masterPointer.enabled = isMaster;
        clientPointer.enabled = !isMaster;

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        // 이 Player를 실제로 조작하는 Client만 충돌 요청
        if (!photonView.IsMine) return;

        IItem item = other.GetComponent<IItem>();

        if (item == null) return;

        PhotonView itemView = other.GetComponent<PhotonView>();

        if (itemView == null) return;

        // Master 자신의 Player가 먹은 경우
        if (PhotonNetwork.IsMasterClient)
        {
            ProcessItemPickup(itemView);
        }
        // 일반 Client가 먹은 경우
        else
        {
            // Client 화면에서는 즉시 사라져 보이게 처리
            HideItemLocally(itemView.gameObject);

            photonView.RPC(nameof(RequestItemPickupRpc), RpcTarget.MasterClient, itemView.ViewID);
            
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    private void HideItemLocally(GameObject itemObject)
    {
        SpriteRenderer[] renderers = itemObject.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        Collider2D[] colliders = itemObject.GetComponentsInChildren<Collider2D>(true);

        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }
    }

    [PunRPC]
    private void RequestItemPickupRpc(int itemViewId, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // 이 Player의 소유자가 실제 요청을 보낸 사람인지 확인
        if (photonView.Owner != info.Sender) return;

        PhotonView itemView = PhotonView.Find(itemViewId);

        // 이미 다른 플레이어가 먹어서 사라졌다면
        if (itemView == null) return;

        ProcessItemPickup(itemView);
    }

    private void ProcessItemPickup(PhotonView itemView)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        IItem item = itemView.GetComponent<IItem>();

        if (item == null) return;

        string itemTag = itemView.gameObject.tag;

        item.Apply(gameObject);

        photonView.RPC(nameof(ShowItemEffectRpc),RpcTarget.All,itemTag);
    }

    [PunRPC]
    private void ShowItemEffectRpc(string itemTag)
    {
        effects.ApplyNetworkEffect(itemTag);
    }
}
