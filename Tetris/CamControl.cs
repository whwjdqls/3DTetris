using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{
    Transform target; // y축
    Transform rotTarget;
    Vector3 lastPos;    // last touch position of mouse

    float sensitivity = 0.25f; // 마우스 감도


    // Start is called before the first frame update
    void Awake()
    {
        rotTarget = transform.parent;
        target = rotTarget.transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        // 항상 카메라가 target을 보고 있도록
        transform.LookAt(target);

        // 마우스 클릭 시 기존 position을 lastPos에 저장
        // 마우스 왼쪽 버튼 클릭 순간
        if(Input.GetMouseButtonDown(0))
        {
            lastPos = Input.mousePosition;
        }

        Orbit();
    }

    void Orbit()
    {
        // 마우스 왼쪽 버튼 클릭 유지
        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastPos;
            float angleY = -delta.y * sensitivity;
            float angleX = delta.x * sensitivity;

            // X axis: camera의 up down 담당
            // Y axis: camera의 양 옆으로의 rotation 담당

            // X axis 기준으로 updown
            Vector3 angles = rotTarget.transform.eulerAngles;
            angles.x += angleY;

            // 위아래의 각도를 -85도~85도로 조절
            angles.x = ClampAngle(angles.x, -85f, 85f);

            rotTarget.transform.eulerAngles = angles;


            // Y axis 기준으로 rotate
            target.RotateAround(target.position, Vector3.up, angleX);
            lastPos = Input.mousePosition;
        }
    }

    // 카메라 위아래로 조절 시 angle 범위 설정.
    // 너무 크면 error
    float ClampAngle(float angle, float from, float to)
    {
        if(angle< 0)
        {
            // negative eulerAngle문제 해결
            angle = 360 + angle;
        }
        if (angle > 180f)
        {
            // angle이 180이 너무 클 때의 문제 해결
            return Mathf.Max(angle, 360 + from);
        }


        return Mathf.Min(angle, to);
    }


}
