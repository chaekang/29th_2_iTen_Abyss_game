using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class FlashlightManager : MonoBehaviour
{
    public static FlashlightManager Instance { get; private set; }

    public Light flashlight; // ������ �Һ�
    public TextMeshProUGUI timerText; // ���͸� Ÿ�̸� UI
    public float flashlightDuration = 480f; // ���͸� ���� �ð� (��)

    private float remainingTime; // ���� ���͸� �ð�
    private bool isTimerRunning = false;

    public bool CanUseFlashlight => remainingTime > 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        GameObject timerObject = GameObject.Find("FlashlightTimer");
        timerText = timerObject?.GetComponent<TextMeshProUGUI>();

        remainingTime = flashlightDuration;
        UpdateTimerUI();

        flashlight.enabled = false;
    }


    public void SetFlashlightState(bool isOn)
    {
        if (!CanUseFlashlight)
        {
            flashlight.enabled = false;
            return;
        }

        flashlight.enabled = isOn;

        if (isOn && !isTimerRunning)
        {
            StartCoroutine(RunBatteryTimer());
        }
    }

    private IEnumerator RunBatteryTimer()
    {
        isTimerRunning = true;

        while (remainingTime > 0 && flashlight.enabled)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimerUI();
            yield return null;
        }

        if (remainingTime <= 0)
        {
            remainingTime = 0;
            flashlight.enabled = false;
            Debug.Log("������ ���͸��� �� ���������ϴ�.");
        }

        isTimerRunning = false;
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        if (timerText != null)
            timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void AddBatteryTime(float amount)
    {
        remainingTime += amount;
        UpdateTimerUI();
        Debug.Log($"���� ���͸� ���� �ð�: {Mathf.FloorToInt(remainingTime / 60)}�� {Mathf.FloorToInt(remainingTime % 60)}��");
    }
}