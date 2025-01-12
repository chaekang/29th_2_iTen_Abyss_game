using UnityEngine;
using Cinemachine;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;

public class SoundMonsterJumpScare : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera jumpScareCam;
    [SerializeField] private PostProcessVolume postProcessing;
    [SerializeField] private GameObject bloodImage;
    [SerializeField] private float scareDuration = 3f;

    [SerializeField] private bool isScaring = false;

    private Animator bloodAni;

    private void Start()
    {
        bloodAni = bloodImage.GetComponent<Animator>();
    }

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

        // ���� ���� ��ٸ�
        yield return new WaitForSeconds(scareDuration - 0.7f);

        // �� Ƣ��� ȿ�� ����
        bloodImage.SetActive(true);
        yield return new WaitForSeconds(0.7f);

        // �Ϲ� ī�޶�� ��ȯ
        jumpScareCam.Priority = 9;

        // ����
        isScaring = false;
    }

    private void Update()
    {
        if (jumpScareCam.Priority >= 11)
        {
            postProcessing.enabled = true;
        }
        else
        {
            postProcessing.enabled = false;
        }
    }
}
