using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    float prevTime; // ���� block�� 1��ŭ ������ ���� �ð�
    float fallTime = 1f; // fallTime ���Ŀ� block�� 1��ŭ ������
    float pointTrackTime = 0.5f;


    // Hand Key points ������ ���� HandTracking scipt ȣ��
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
        // Manager���� HandTracking scipt ��������
        // exe ���Ͽ��� udp ����� ���� �� key point�� ��ǥ�� ������ �� �ʼ�
        

            


        // �ð��� fallTime��ŭ ������ -> block�� 1 ��ŭ �̵�
        if (Time.time - prevTime > fallTime)
        {
            transform.position += Vector3.down;
            
            // IF BLOCK IS NOT IN THE GRID
            if (!CheckValidMove())
            {
                
                transform.position += Vector3.up; // transform.position += Vector3.down�� �ݴ�� ���� �־� -> �ᱹ ���ڸ�

                // DELETE LAYER IF POSSIBLE
                Playfield.instance.DeleteLayer();

                // DISABLES THIS TETRISBLOCK.CS SCRIPT WHEN LANDING ON THE BOTTON GRID
                enabled = false;


                // CREATE A NEW BLOCK
                // Grid �ȿ� �����̴� tetris�� ���� �� spawn

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

        // ����Ű�� tetris block �����̱�
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
        // HandTracking.points.Length - 1: points�� ���� ������ ����
        // ��, float.Parse(HandTracking.points[HandTracking.points.Length - 1]) ��
        // points�� ���� ������ ���Ҹ� float���� ������ ��
        // ������: �ѹ��� 27���� label�� ������
        // sol: 27�� ī��Ʈ �Ͽ� 27���縶�� 1�� �ش� �������� �̵�
        print(rcv_points[rcv_points.Length - 1]);

        // Unity������ I==27
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


        // ȸ����Ű��
        if (Input.GetKeyDown(KeyCode.W))
        {
            // x�� ���� 90��
            SetRotationInput(new Vector3(90, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            // x�� ���� -90��
            SetRotationInput(new Vector3(-90, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            // x�� ���� 90��
            SetRotationInput(new Vector3(0, 0, 90));
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            // x�� ���� -90��
            SetRotationInput(new Vector3(0, 0, -90));
        }


    }

    // Tetris block �̵� �Լ�
    public void SetInput(Vector3 direction)
    {
        // tetris�� position�� direction ��ŭ ���ؼ� �̵���Ų��
        transform.position += direction;

        // Grid�� �ٸ� tetris �� ������ �� ���� ���� �ִٸ�
        if (!CheckValidMove())
        {
            // �ٽ� direction��ŭ ���ش�
            // Debug.Log("False");
            transform.position -= direction;
        }
        else
        {
            // �������� �����ϴٸ� �� ��ġ�� �� tetris block�� �������ش�.
            // this = �� tetris block
            Playfield.instance.UpdateGrid(this);
        }
    }

    
    // Tetris block ȸ�� �Լ�
    public void SetRotationInput(Vector3 rotation)
    {
        transform.Rotate(rotation, Space.World);

        // Grid���� rotate �� �� ���� ��Ȳ�̸� �ٽ� ���� -rotation��ŭ rotate
        if(!CheckValidMove())
        {
            transform.Rotate(-rotation, Space.World);
        }
        else
        {
            // ȸ���� �����ϴٸ� �� ��ġ�� �� tetris block�� ȸ�������ش�.
            // this = �� tetris block
            Playfield.instance.UpdateGrid(this);
        }
    }

    bool CheckValidMove()
    {
        // child = �� tetris block�� cube
        // in transform: in Tetris block
        foreach (Transform child in transform)
        {
            // Playfield.cs���� Round()�� �ҷ���
            // ��� ��ǥ�� �ݿø��Ͽ� ����ȭ
            Vector3 pos = Playfield.instance.Round(child.position);
            

            // Playfield.cs���� CheckInsideGrid()�� �ҷ���
            // block�� grid �ȿ� ������ -> return false
            if (!Playfield.instance.CheckInsideGrid(pos))
            {
                return false;
            }

        }

        // �� foreach�� ���� grid �ȿ� ������ Ȯ���ϸ�
        // �Ʒ� foreach�� ���� �ٸ� tetris�� ��ġ�� �ʵ��� �Ѵ�
        foreach(Transform child in transform)
        {
            // pos = �� child�� ��ǥ
            Vector3 pos = Playfield.instance.Round(child.position);
            Transform t = Playfield.instance.GetTransformOnGridPos(pos);
            // � cube�� ��ǥ�� �ִµ� ���� tetris�� ���� cube�� �ƴ϶��
            // �������� �ʵ��� return false
            if (t != null && t.parent != transform)
            {
                return false;
            }

        }


        return true;
    }
}
