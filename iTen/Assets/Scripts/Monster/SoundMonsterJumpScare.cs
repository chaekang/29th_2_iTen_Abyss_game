using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;
using System.Net.NetworkInformation;
using System.Collections;

public class SoundMonsterJumpScare : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera jumpScareCam;
    [SerializeField] private Animator cameraAni;
    [SerializeField] private float scareDuration = 3f;

    [SerializeField] private bool isScaring = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster") && !isScaring)
        {
            Debug.Log("Start JumpScare");
            StartCoroutine(TriggerJumpScare());
        }
    }

    private IEnumerator TriggerJumpScare()
    {
        isScaring = true;

        // �������ɾ� ī�޶� Ȱ��ȭ
        jumpScareCam.Priority = 11;

        // ī�޶� �ִϸ��̼� ����
        if (cameraAni != null)
        {
            cameraAni.SetTrigger("PlayJumpScare");
        }

        // ���� ���� ��ٸ�
        yield return new WaitForSeconds(scareDuration);

        // �Ϲ� ī�޶�� ��ȯ
        jumpScareCam.Priority = 9;

        // ����
        isScaring = false;
    }
}
