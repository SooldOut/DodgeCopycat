using UnityEngine;

public class Diamond : MonoBehaviour
{
    // OnTriggerEnter는 Is Trigger가 켜진 콜라이더와 충돌을 감지합니다.


    // 이 함수는 매 프레임마다 호출됩니다.
    void Update()
    {
        // 다이아몬드의 X 회전값을 90도로 고정합니다.
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.x = 90f;
        transform.eulerAngles = currentRotation;
    }
}