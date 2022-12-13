using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    float prevTime; // 직전 block이 1만큼 떨어진 후의 시간
    float fallTime = 1f; // fallTime 이후에 block이 1만큼 떨어짐
    float pointTrackTime = 0.5f;


    // Hand Key points 값들을 가진 HandTracking scipt 호출
    // HandTracking handTracking;

    

    int f = 0;
    int b = 0;
    int r = 0;
    int l = 0;

    // public UDPReceive udpReceive;
    HandTracking handTracking;

    private string[] rcv_points;
    void Start()
    {
        handTracking = GameObject.Find("Manager").GetComponent<HandTracking>();
    }

    // Update is called once per frame
    void Update()
    {
        rcv_points = handTracking.SendPoints();
        // Manager에서 HandTracking scipt 가져오기
        // exe 파일에서 udp 통신을 통해 각 key point의 좌표를 가져올 때 필수
        

            


        // 시간이 fallTime만큼 지나면 -> block이 1 만큼 이동
        if (Time.time - prevTime > fallTime)
        {
            transform.position += Vector3.down;
            
            // IF BLOCK IS NOT IN THE GRID
            if (!CheckValidMove())
            {
                
                transform.position += Vector3.up; // transform.position += Vector3.down의 반대로 힘을 주어 -> 결국 제자리

                // DELETE LAYER IF POSSIBLE
                Playfield.instance.DeleteLayer();

                // DISABLES THIS TETRISBLOCK.CS SCRIPT WHEN LANDING ON THE BOTTON GRID
                enabled = false;


                // CREATE A NEW BLOCK
                // Grid 안에 움직이는 tetris가 없을 때 spawn

                Playfield.instance.SpawnNewBlock();
               




            }

            // IF BLOCK IS IN THE GIRD
            else
            {
                // UPDATE THE GRID
                // this = current block
                Playfield.instance.UpdateGrid(this);
            }
            
            prevTime = Time.time;

        }

        // 방향키로 tetris block 움직이기
        /*
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetInput(Vector3.left);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetInput(Vector3.right);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetInput(Vector3.forward);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetInput(Vector3.back);
        }
        
        */


        // float.Parse(string): string -> float
        // HandTracking.points.Length - 1: points의 가장 마지막 원소
        // 즉, float.Parse(HandTracking.points[HandTracking.points.Length - 1]) 은
        // points의 가장 마지막 원소를 float으로 가져온 것
        // 문제점: 한번에 27개의 label을 가져옴
        // sol: 27번 카운트 하여 27번재마다 1씩 해당 방향으로 이동
        print(rcv_points[rcv_points.Length - 1]);

        // Unity에서는 I==27
        if (float.Parse(rcv_points[rcv_points.Length - 1]) == 1.0)
        {
            
            
            l += 1;
            if (l == 5)
            {
                SetInput(Vector3.left);
                l = 0;
            }
        }
        if (float.Parse(rcv_points[rcv_points.Length - 1]) == 3.0)
        {
            
            
            r += 1;
            if (r == 5)
            {
                SetInput(Vector3.right);
                r = 0;
            }
        }
        if (float.Parse(rcv_points[rcv_points.Length - 1]) == 2.0)
        {
            
            
            f += 1;
            if (f == 5)
            {
                SetInput(Vector3.forward);
                f = 0;
            }

        }
        if (float.Parse(rcv_points[rcv_points.Length - 1]) == 4.0)
        {
      
            
            b += 1;
            if (b == 5)
            {
                SetInput(Vector3.back);
                b = 0;
            }
        }
        /*
        print(handTracking.points[handTracking.points.Length - 1]);

        if (float.Parse(handTracking.points[handTracking.points.Length - 1]) == 1.0)
        {
            l += 1;
            if (l == 27)
            {
                SetInput(Vector3.left);
                l = 0;
            }
        }
        if (float.Parse(handTracking.points[handTracking.points.Length - 1]) == 3.0)
        {
            r += 1;
            if (r == 27)
            {
                SetInput(Vector3.right);
                r = 0;
            }
        }
        if (float.Parse(handTracking.points[handTracking.points.Length - 1]) == 2.0)
        {
            f += 1;
            if (f == 27)
            {
                SetInput(Vector3.forward);
                f = 0;
            }
          
        }
        if (float.Parse(handTracking.points[handTracking.points.Length - 1]) == 4.0)
        {
            b += 1;
            if (b == 27)
            {
                SetInput(Vector3.back);
                b = 0;
            }
        }
        */


        // 회전시키기
        if (Input.GetKeyDown(KeyCode.W))
        {
            // x축 기준 90도
            SetRotationInput(new Vector3(90, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            // x축 기준 -90도
            SetRotationInput(new Vector3(-90, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            // x축 기준 90도
            SetRotationInput(new Vector3(0, 0, 90));
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            // x축 기준 -90도
            SetRotationInput(new Vector3(0, 0, -90));
        }


    }

    // Tetris block 이동 함수
    public void SetInput(Vector3 direction)
    {
        // tetris의 position에 direction 만큼 더해서 이동시킨다
        transform.position += direction;

        // Grid나 다른 tetris 등 움직일 수 없는 벽이 있다면
        if (!CheckValidMove())
        {
            // 다시 direction만큼 빼준다
            // Debug.Log("False");
            transform.position -= direction;
        }
        else
        {
            // 움직임이 가능하다면 그 위치에 이 tetris block을 움직여준다.
            // this = 현 tetris block
            Playfield.instance.UpdateGrid(this);
        }
    }

    
    // Tetris block 회전 함수
    public void SetRotationInput(Vector3 rotation)
    {
        transform.Rotate(rotation, Space.World);

        // Grid에서 rotate 할 수 없는 상황이면 다시 도로 -rotation만큼 rotate
        if(!CheckValidMove())
        {
            transform.Rotate(-rotation, Space.World);
        }
        else
        {
            // 회전이 가능하다면 그 위치에 이 tetris block을 회전시켜준다.
            // this = 현 tetris block
            Playfield.instance.UpdateGrid(this);
        }
    }

    bool CheckValidMove()
    {
        // child = 각 tetris block의 cube
        // in transform: in Tetris block
        foreach (Transform child in transform)
        {
            // Playfield.cs에서 Round()를 불러와
            // 모든 좌표를 반올림하여 정수화
            Vector3 pos = Playfield.instance.Round(child.position);
            

            // Playfield.cs에서 CheckInsideGrid()를 불러와
            // block이 grid 안에 없으면 -> return false
            if (!Playfield.instance.CheckInsideGrid(pos))
            {
                return false;
            }

        }

        // 위 foreach를 통해 grid 안에 있음을 확인하면
        // 아래 foreach를 통해 다른 tetris와 겹치지 않도록 한다
        foreach(Transform child in transform)
        {
            // pos = 각 child의 좌표
            Vector3 pos = Playfield.instance.Round(child.position);
            Transform t = Playfield.instance.GetTransformOnGridPos(pos);
            // 어떤 cube가 좌표에 있는데 현재 tetris에 속한 cube가 아니라면
            // 움직이지 않도록 return false
            if (t != null && t.parent != transform)
            {
                return false;
            }

        }


        return true;
    }
}
