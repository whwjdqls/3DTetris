using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ghostBlock : MonoBehaviour
{
    // parent = tetris block
    GameObject parent;
    // parentTetris = tetris block의 실제 cube들
    TetrisBlock parentTetris;


    // Start is called before the first frame update
    void Start()
    {
        // StartCoroutine?? -> tetris block을 따라갈 수 있는 이유인가?
        StartCoroutine(RepositionBlock());
    }

    public void SetParent(GameObject _parent)
    {
        // Ghost와 실제 block들을 가져옴
        parent = _parent;
        parentTetris = parent.GetComponent<TetrisBlock>();    

    }

    void PositionGhost()
    {
        // transform.position: ghost의 position
        // parent.transform.position: tetris block의 position
        transform.position = parent.transform.position;
        transform.rotation = parent.transform.rotation;
    }

    IEnumerator RepositionBlock()
    {
        // Tetris block이 생성되어 바닥에 닿기 전까지
        while (parentTetris.enabled)
        {
            // Ghost block의 위치 및 회전 정도 가져옴
            PositionGhost();

            // Tetris Block 아래에 땅 위에 위치하게 설정
            MoveDown();

            // ??
            yield return new WaitForSeconds(0.1f);
        }
        // Tetris block이 바닥에 닿았을 때 -> ghost block도 제거
        Destroy(gameObject);
        yield return null;
    }


    void MoveDown()
    {
        // Time.time 등이 쓰이지 않았으므로
        // Ghost block은 무한정 내려가다가
        // 바닥에 도달하면 계속 바닥에 있는다.
        // 그래서 ghost의 역할을 할 수 있게 된다.
        while (CheckValidMove())
        {
            transform.position += Vector3.down;

        }
        if (!CheckValidMove())
        {
            transform.position += Vector3.up;
        }
    }

    // Tetris block의 CheckValidMove와 매우 유사
    bool CheckValidMove()
    {
        // child = 각 ghost block의 cube
        // in transform: in Ghost block
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
        // in transform: in Ghost block
        foreach (Transform child in transform)
        {
            // pos = 각 child의 좌표
            Vector3 pos = Playfield.instance.Round(child.position);
            Transform t = Playfield.instance.GetTransformOnGridPos(pos);

            // Ghost Block 끼리 겹치지거나 다른 tetris block과 겹치지 않도록
            if (t != null && t.parent == parent.transform)
            {
                return true;
            }

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