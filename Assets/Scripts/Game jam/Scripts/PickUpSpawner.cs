using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSpawner : MonoBehaviour
{
    [SerializeField] private GameObject coinPickUpPrefab, healthPickUpPrefab;

    public void DropPickUp()
    {
        int random = Random.Range(1, 5);

        if (random == 1)
        {
            Instantiate(healthPickUpPrefab, transform.position, Quaternion.identity);
            Debug.Log("Health");
        }
        Instantiate(coinPickUpPrefab, transform.position, Quaternion.identity);
    }
}