using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class SpawnPlayerJoin : MonoBehaviourPunCallbacks
{
    public GameObject playerInfoPrefab;
    public float x;
    public float y;

    private void Start()
    {
        Vector2 spawnPosition = new Vector2(x, y);
        PhotonNetwork.Instantiate(playerInfoPrefab.name, spawnPosition, Quaternion.identity);
    }
}
