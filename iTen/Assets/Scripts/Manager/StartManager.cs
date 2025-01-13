using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    // �� �Ŵ����� �̱����� �ƴ� �ܼ��� ī�޶�� �÷��̾ �����ϴ� ������ �ϴ� �Ŵ���
    // StartManager��� ���ص� �Ǵ� ��� ����

    [SerializeField] private CinemachineVirtualCamera _camera;

    private void Start()
    {
        // �÷ο� �ܰ�
        // 1. ���� ȯ�� ���� �� ���� �� �ϱ�

        // 2. ĳ���� ����
        SpawnManager spawnMawnager = SpawnManager.Instance;
        spawnMawnager.spawn.SpawnPlayer();

        // 3. ī�޶�� ���� �����ֱ�
        _camera.Follow = spawnMawnager.spawn.player.GetComponent<FirstPersonController>().FollowTransform;
    }
}
