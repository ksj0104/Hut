using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Vector3 spawnPosition;  // 플레이어가 생성될 위치
    public Quaternion spawnRotation = Quaternion.identity;  // 기본 회전값

    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            // Player Prefab을 네트워크 상에서 생성
        }
    }
}