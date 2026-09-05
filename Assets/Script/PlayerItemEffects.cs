using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemEffects : MonoBehaviourPun
{
    [SerializeField]
    private GameObject itemUI;

    [SerializeField]
    private Text itemText;

    [SerializeField]
    private AudioClip itemClip;

    [SerializeField]
    private AudioClip iceClip;

    [SerializeField]
    private AudioClip boomClip;

    private AudioSource audioSource;
    private PlayerMovement movement;

    private Coroutine speedCoroutine;

    private Coroutine messageCoroutine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        movement = GetComponent<PlayerMovement>();
        
        itemUI.SetActive(false);
    }

    public void ApplyNetworkEffect(string itemTag)
    {
        switch (itemTag)
        {
            case "speedUp":
                ShowMessage("SPEED UP", 2f);

                if (photonView.IsMine)
                {
                    ApplySpeedUp();
                }

                break;


            case "ice":
                ShowMessage("Frozen!", 3f);

                if (photonView.IsMine)
                {
                    ApplyIce();
                }

                break;


            case "multiScore":
                ShowMessage("SCORE x2", 5f);

                if (photonView.IsMine)
                {
                    audioSource.PlayOneShot(itemClip);
                }

                break;


            case "boom":
                ShowMessage("BOOM", 1.5f);

                if (photonView.IsMine)
                {
                    audioSource.PlayOneShot(boomClip);
                }

                break;
        }
    }

    public void ApplySpeedUp()
    {
        if (!photonView.IsMine) return;

        if (speedCoroutine != null)
        {
            StopCoroutine(speedCoroutine);
        }

        speedCoroutine = StartCoroutine(ChangeSpeedRoutine(6f, 2f));

        audioSource.PlayOneShot(itemClip);
    }

    public void ApplyIce()
    {
        if (!photonView.IsMine) return;

        if (speedCoroutine != null)
        {
            StopCoroutine(speedCoroutine);
        }

        speedCoroutine = StartCoroutine(ChangeSpeedRoutine(0f, 3f));

        audioSource.PlayOneShot(iceClip);
    }

    private void ShowMessage(string message, float duration)
    {
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
        }

        messageCoroutine = StartCoroutine(ShowMessageRoutine(message, duration));
    }

    private IEnumerator ShowMessageRoutine(string message, float duration)
    {
        itemText.text = message;
        itemUI.SetActive(true);

        yield return new WaitForSeconds(duration);

        itemUI.SetActive(false);

        messageCoroutine = null;
    }

    private IEnumerator ChangeSpeedRoutine(float speed, float duration)
    {
        movement.SetSpeed(speed);

        yield return new WaitForSeconds(duration);

        movement.ResetSpeed();
        speedCoroutine = null;
    }
}