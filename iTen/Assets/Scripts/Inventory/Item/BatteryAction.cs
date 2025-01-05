using UnityEngine;

[CreateAssetMenu(fileName = "New BatteryAction", menuName = "Inventory/Actions/BatteryAction")]
public class BatteryAction : ItemAction
{
    public override void Use(GameObject user)
    {
        FlashlightAction flashlight = FindObjectOfType<FlashlightAction>();
        flashlight.AddBattery(120f);

        Debug.Log("���͸��� ���Ǿ����ϴ�. ����� 2�� �߰�.");
    }
}