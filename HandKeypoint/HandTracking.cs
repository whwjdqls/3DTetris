using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HandTracking : MonoBehaviour
{
    // Get data from UDPReceived -> assign to all 21 points
    // UDPReceive.cs�� �ҷ���
    public UDPReceive udpReceive;
    // public TCPReceive tcpReceive;

    // 21���� points �ҷ���
    public GameObject[] handPoints_1;
    public GameObject[] handPoints_2;

    // �ٸ� script�� ��� ������ ��ǥ�� �����Ͽ� tetris block�� ��������
    // only right hand
    // static���� TetrisBlock.cs���� GetComponent<> ���� ���� ����
    public string[] points;

    void Start()
    {
        
    }

    public string[] SendPoints()
    {
        return points;
    }

    // Update is called once per frame
    void Update()
    {
        // �� point�� ������ data�� 3���� ����
        // However, data�� list�̹Ƿ� string���� ��ȯ�� []�� �� ���� ���� -> ��������
        // ���� ������ ','�� ����

        // [] ����
        string data = udpReceive.data;
        data = data.Remove(0, 1); // ���� ��(=0)���� 1���� char ���� == '['
        data = data.Remove(data.Length-1, 1); // ���� �ڿ��� 1���� char ���� == ']', 0���� �����̹Ƿ� Length-1

        // ',' ����
        points = data.Split(',');
        //points = data.Split(',');

        // ���� points�� 21*3 ���� ���ڰ� ��

        // Send to handPoints
        // ��, string���� float�� �ٲ��ֱ�
        // int�� �ƴ� float�̾�� Unity�󿡼� handPoints�� ������ �����̵��� �� �� ����

        // �� ���̶� �νĵǸ�
        
        for (int i = 0; i < 21; i++)
        {
            // 3���� ���� ���� webcam���� ������ data�� ��� ����̰�
            // Unity�� game view�� ����� �����̹Ƿ�
            // game view�󿡼� x�� ���������δ� ���� ������ �� ����
            // �׷��� 5���� �������ν� game view ��ü�� ��� ����

            // 100���� ������ ��: webcam���� �޾ƿ��� data�� ũ�Ⱑ unity���� ��ǥ���� �ʹ� ũ��
            // �����̴� ������ ������ ������ 100 ��� �� ���� ���� �����ָ� ��
            // float.Parse(string): string -> float
            
            float x = 3 - float.Parse(points[i * 3]) / 100;
            // x1 y1, z1, x2, y2, z2. x3, y3, z3
            // 0          1*3         2*3
            float y = float.Parse(points[i * 3 + 1]) / 100;
            float z = float.Parse(points[i * 3 + 2]) / 100;

            // �� ��ǥ�� �� Point�� ���
            // Pycharm���� �޾ƿ��� data�� ������ Point�� ������ ������ ��
            handPoints_1[i].transform.localPosition = new Vector3(x, y, z);
        
        }
        
       

        // ���� keypoints ��ǥ�� data ���̰� 63+1���� ��ٴ� ���� (1�� label)
        // ���� 2���� �νĵǾ��ٴ� ��
        if (points.Length  > 64)
        //if (points.Length  > 64)
        {
            for (int i = 0; i < 21; i++)
            {

                float x2 = 3 - float.Parse(points[i * 3 + 63]) / 100;

                float y2 = float.Parse(points[i * 3 + 1 + 63]) / 100;
                float z2 = float.Parse(points[i * 3 + 2 + 63]) / 100;

                handPoints_2[i].transform.localPosition = new Vector3(x2, y2, z2);
            }
        }
        // ���� �Ѱ��� �ν� �Ǹ� -> ������ ���� keypoints ���� camera FoV���� �������
        else
        {   

            for (int i = 0; i < 21; i++)
                handPoints_2[i].transform.localPosition = new Vector3(-1.4f, 0f, -17.91f);
        }





        // print(points[points.Length-1]);
    }
}
