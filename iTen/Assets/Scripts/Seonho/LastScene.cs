using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LastScene : MonoBehaviour
{
    public string MainScene;

    private float delayTime = 20.4f;

    void Start()
    {
        StartCoroutine(TransitionAfterDelay());
    }

    private IEnumerator TransitionAfterDelay()
    {
        yield return new WaitForSeconds(delayTime);

        if (!string.IsNullOrEmpty(MainScene))
        {
            SceneManager.LoadScene(MainScene);
        }
        else
        {
            Debug.LogError("���� �� �̸��� �������� �ʾҽ��ϴ�.");
        }
    }
}
