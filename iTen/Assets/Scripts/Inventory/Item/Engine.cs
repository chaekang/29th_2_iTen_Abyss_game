using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    private bool isWireConnected = false;

    public void ConnectWire()
    {
        if (!isWireConnected)
        {
            isWireConnected = true;
            Debug.Log("������ ������ ����Ǿ����ϴ�.");
            // ���� ���� ���� �ڵ� �߰�
        }
        else
        {
            Debug.Log("�̹� ������ ����Ǿ� �ֽ��ϴ�.");
        }
    }
}