using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{
    Transform target; // y��
    Transform rotTarget;
    Vector3 lastPos;    // last touch position of mouse

    float sensitivity = 0.25f; // ���콺 ����


    // Start is called before the first frame update
    void Awake()
    {
        rotTarget = transform.parent;
        target = rotTarget.transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        // �׻� ī�޶� target�� ���� �ֵ���
        transform.LookAt(target);

        // ���콺 Ŭ�� �� ���� position�� lastPos�� ����
        // ���콺 ���� ��ư Ŭ�� ����
        if(Input.GetMouseButtonDown(0))
        {
            lastPos = Input.mousePosition;
        }

        Orbit();
    }

    void Orbit()
    {
        // ���콺 ���� ��ư Ŭ�� ����
        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastPos;
            float angleY = -delta.y * sensitivity;
            float angleX = delta.x * sensitivity;

            // X axis: camera�� up down ���
            // Y axis: camera�� �� �������� rotation ���

            // X axis �������� updown
            Vector3 angles = rotTarget.transform.eulerAngles;
            angles.x += angleY;

            // ���Ʒ��� ������ -85��~85���� ����
            angles.x = ClampAngle(angles.x, -85f, 85f);

            rotTarget.transform.eulerAngles = angles;


            // Y axis �������� rotate
            target.RotateAround(target.position, Vector3.up, angleX);
            lastPos = Input.mousePosition;
        }
    }

    // ī�޶� ���Ʒ��� ���� �� angle ���� ����.
    // �ʹ� ũ�� error
    float ClampAngle(float angle, float from, float to)
    {
        if(angle< 0)
        {
            // negative eulerAngle���� �ذ�
            angle = 360 + angle;
        }
        if (angle > 180f)
        {
            // angle�� 180�� �ʹ� Ŭ ���� ���� �ذ�
            return Mathf.Max(angle, 360 + from);
        }


        return Mathf.Min(angle, to);
    }


}
