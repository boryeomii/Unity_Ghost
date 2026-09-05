using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class boneSpawner : MonoBehaviourPun
{
    public GameObject bone;

    private float timeBetSpawnMax = 1f;
    private float timeBetSpawnMin = 0.5f;
    private float timeBetSpawn;

    private float xPos;
    private float yPos;

    private float lastSpawnTime;

    // Start is called before the first frame update
    void Start()
    {
        timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
        lastSpawnTime = 0;      
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient || GameManager.instance.isGameover) return;

        if (Time.time >= lastSpawnTime + timeBetSpawn)
        {
            lastSpawnTime = Time.time;
            timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);

            xPos = Random.Range(-7, 8);
            yPos = Random.Range(-3, 4);

            Spawn(xPos, yPos);
        }
    }

    private void Spawn(float xPos, float yPos)
    {
        Vector2 spawnPosition = new Vector2(xPos, yPos);
        GameObject item = PhotonNetwork.Instantiate(bone.name, spawnPosition, Quaternion.identity);
        StartCoroutine(DestroyAfter(item, 5f));
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
