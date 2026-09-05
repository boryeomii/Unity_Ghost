using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class fireSpawner : MonoBehaviourPun
{
    public GameObject firePrefab;

    public float spawnRateMin = 1f;
    public float spawnRateMax = 7f;

    private Transform target;

    private float spawnRate;
    private float timeAfterSpawn;

    public GameObject[] points;
    GameObject selectedPoint;

    // Start is called before the first frame update
    void Start()
    {
        timeAfterSpawn = 0f;
        selectedPoint = points[Random.Range(0, points.Length)];
        target = selectedPoint.transform;
        spawnRate = Random.Range(spawnRateMin, spawnRateMax);
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient || GameManager.instance.isGameover) return;

        timeAfterSpawn += Time.deltaTime;

        if (timeAfterSpawn > spawnRate && GameManager.instance.RemainingTime <= 30f)
        {
            timeAfterSpawn = 0f;
            CreateFire();
        }
    }

    public void CreateFire()
    {
        selectedPoint = points[Random.Range(0, points.Length)];
        target = selectedPoint.transform;

        GameObject fireObject = PhotonNetwork.Instantiate(firePrefab.name, transform.position, Quaternion.identity);
        fire fireComponent = fireObject.GetComponent<fire>();

        fireComponent.Initialize(target.position);

        spawnRate = Random.Range(spawnRateMin, spawnRateMax);

        StartCoroutine(DestroyAfter(fireObject, 10f));
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
