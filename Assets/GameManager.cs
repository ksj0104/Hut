using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Vector3 spawnPosition;  // �÷��̾ ������ ��ġ
    public Quaternion spawnRotation = Quaternion.identity;  // �⺻ ȸ����

    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            // Player Prefab�� ��Ʈ��ũ �󿡼� ����
        }
    }
}