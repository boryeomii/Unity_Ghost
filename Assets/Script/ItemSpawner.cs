using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemSpawner : MonoBehaviourPun
{   
    public GameObject[] normalItems;
    public GameObject[] hardItems;

    public float timeBetSpawnMax = 7f;
    public float timeBetSpawnMin = 2f;
    private float normalSpawnInterval;
    private float hardSpawnInterval;

    private float xPos;
    private float yPos;

    private float normalLastSpawn;
    private float hardLastSpawn;

    // Start is called before the first frame update
    void Start()
    {
        normalSpawnInterval = Random.Range(timeBetSpawnMin,timeBetSpawnMax);
        hardSpawnInterval = Random.Range(timeBetSpawnMin,timeBetSpawnMax);
        normalLastSpawn = 0;
        hardLastSpawn = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient || GameManager.instance.isGameover) return;

        if (Time.time >= normalLastSpawn + normalSpawnInterval)
        {
            normalLastSpawn = Time.time;
            normalSpawnInterval = Random.Range(timeBetSpawnMin, timeBetSpawnMax);

            xPos = Random.Range(-7, 8);
            yPos = Random.Range(-3, 4);

            NormalSpawn(xPos, yPos);
        }

        if (Time.time >= hardLastSpawn + hardSpawnInterval && GameManager.instance.RemainingTime <= 60f)
        {
            hardLastSpawn = Time.time;
            hardSpawnInterval = Random.Range(timeBetSpawnMin, timeBetSpawnMax);

            xPos = Random.Range(-7, 8);
            yPos = Random.Range(-3, 4);

            HardSpawn(xPos, yPos);
        }
    }

    private void NormalSpawn(float xPos,  float yPos)
    {
        Vector2 spawnPosition=new Vector2(xPos,yPos);

        GameObject firstSelected = normalItems[Random.Range(0, normalItems.Length)];
        GameObject firstItem = PhotonNetwork.Instantiate(firstSelected.name, spawnPosition, Quaternion.identity);

        StartCoroutine(DestroyAfter(firstItem, 5f));
    }

    private void HardSpawn(float xPos, float yPos)
    {
        Vector2 spawnPosition = new Vector2(xPos, yPos);

        GameObject secondSelected = hardItems[Random.Range(0, hardItems.Length)];
        GameObject secondItem = PhotonNetwork.Instantiate(secondSelected.name, spawnPosition, Quaternion.identity);

        StartCoroutine(DestroyAfter(secondItem, 5f));
    }

    IEnumerator DestroyAfter(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (target != null)
        {
            PhotonNetwork.Destroy(target);
        }
    }
}
