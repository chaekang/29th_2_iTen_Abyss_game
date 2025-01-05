using UnityEngine;

[CreateAssetMenu(fileName = "New WireAction", menuName = "Inventory/Actions/WireAction")]
public class WireAction : ItemAction
{
    public override void Use(GameObject user)
    {
        Engine engine = FindObjectOfType<Engine>();
        if (engine != null)
        {
            engine.ConnectWire();
            Debug.Log("������ ������ ����Ǿ����ϴ�.");
        }
        else
        {
            Debug.Log("������ ã�� �� �����ϴ�.");
        }
    }
}