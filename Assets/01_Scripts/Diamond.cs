using UnityEngine;

public class Diamond : MonoBehaviour
{
    // OnTriggerEnter�� Is Trigger�� ���� �ݶ��̴��� �浹�� �����մϴ�.


    // �� �Լ��� �� �����Ӹ��� ȣ��˴ϴ�.
    void Update()
    {
        // ���̾Ƹ���� X ȸ������ 90���� �����մϴ�.
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.x = 90f;
        transform.eulerAngles = currentRotation;
    }
}