using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HandTracking : MonoBehaviour
{
    // Get data from UDPReceived -> assign to all 21 points
    // UDPReceive.cs를 불러옴
    public UDPReceive udpReceive;
    // public TCPReceive tcpReceive;

    // 21개의 points 불러옴
    public GameObject[] handPoints_1;
    public GameObject[] handPoints_2;

    // 다른 script에 모든 점들의 좌표를 전달하여 tetris block을 움직이자
    // only right hand
    // static으로 TetrisBlock.cs에서 GetComponent<> 없이 접근 가능
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
        // 각 point에 가져온 data를 3개씩 배정
        // However, data는 list이므로 string으로 전환시 []가 양 끝에 존재 -> 삭제하자
        // 숫자 사이의 ','도 제거

        // [] 제거
        string data = udpReceive.data;
        data = data.Remove(0, 1); // 가장 앞(=0)에서 1개의 char 제거 == '['
        data = data.Remove(data.Length-1, 1); // 가장 뒤에서 1개의 char 제거 == ']', 0부터 시작이므로 Length-1

        // ',' 제거
        points = data.Split(',');
        //points = data.Split(',');

        // 이제 points에 21*3 개의 숫자가 들어감

        // Send to handPoints
        // 단, string에서 float로 바꿔주기
        // int가 아닌 float이어야 Unity상에서 handPoints가 세세히 움직이도록 할 수 있음

        // 한 손이라도 인식되면
        
        for (int i = 0; i < 21; i++)
        {
            // 3에서 빼는 것은 webcam에서 들어오는 data가 모두 양수이고
            // Unity의 game view상 가운데는 원점이므로
            // game view상에서 x의 음수쪽으로는 손을 움직일 수 없다
            // 그래서 5에서 빼줌으로써 game view 전체를 사용 가능

            // 100으로 나누는 것: webcam에서 받아오는 data의 크기가 unity상의 좌표보다 너무 크다
            // 움직이는 범위를 넓히고 싶으면 100 대신 더 작은 수로 나눠주면 됨
            // float.Parse(string): string -> float
            
            float x = 3 - float.Parse(points[i * 3]) / 100;
            // x1 y1, z1, x2, y2, z2. x3, y3, z3
            // 0          1*3         2*3
            float y = float.Parse(points[i * 3 + 1]) / 100;
            float z = float.Parse(points[i * 3 + 2]) / 100;

            // 각 좌표를 각 Point에 배당
            // Pycharm에서 받아오는 data의 순서와 Point의 순서가 동일한 듯
            handPoints_1[i].transform.localPosition = new Vector3(x, y, z);
        
        }
        
       

        // 받은 keypoints 좌표의 data 길이가 63+1보다 길다는 것은 (1은 label)
        // 손이 2개가 인식되었다는 것
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
        // 손이 한개만 인식 되면 -> 나머지 손의 keypoints 들은 camera FoV에서 사라지게
        else
        {   

            for (int i = 0; i < 21; i++)
                handPoints_2[i].transform.localPosition = new Vector3(-1.4f, 0f, -17.91f);
        }





        // print(points[points.Length-1]);
    }
}
