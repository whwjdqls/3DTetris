using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ghostBlock : MonoBehaviour
{
    // parent = tetris block
    GameObject parent;
    // parentTetris = tetris block�� ���� cube��
    TetrisBlock parentTetris;


    // Start is called before the first frame update
    void Start()
    {
        // StartCoroutine?? -> tetris block�� ���� �� �ִ� �����ΰ�?
        StartCoroutine(RepositionBlock());
    }

    public void SetParent(GameObject _parent)
    {
        // Ghost�� ���� block���� ������
        parent = _parent;
        parentTetris = parent.GetComponent<TetrisBlock>();    

    }

    void PositionGhost()
    {
        // transform.position: ghost�� position
        // parent.transform.position: tetris block�� position
        transform.position = parent.transform.position;
        transform.rotation = parent.transform.rotation;
    }

    IEnumerator RepositionBlock()
    {
        // Tetris block�� �����Ǿ� �ٴڿ� ��� ������
        while (parentTetris.enabled)
        {
            // Ghost block�� ��ġ �� ȸ�� ���� ������
            PositionGhost();

            // Tetris Block �Ʒ��� �� ���� ��ġ�ϰ� ����
            MoveDown();

            // ??
            yield return new WaitForSeconds(0.1f);
        }
        // Tetris block�� �ٴڿ� ����� �� -> ghost block�� ����
        Destroy(gameObject);
        yield return null;
    }


    void MoveDown()
    {
        // Time.time ���� ������ �ʾ����Ƿ�
        // Ghost block�� ������ �������ٰ�
        // �ٴڿ� �����ϸ� ��� �ٴڿ� �ִ´�.
        // �׷��� ghost�� ������ �� �� �ְ� �ȴ�.
        while (CheckValidMove())
        {
            transform.position += Vector3.down;

        }
        if (!CheckValidMove())
        {
            transform.position += Vector3.up;
        }
    }

    // Tetris block�� CheckValidMove�� �ſ� ����
    bool CheckValidMove()
    {
        // child = �� ghost block�� cube
        // in transform: in Ghost block
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
        // in transform: in Ghost block
        foreach (Transform child in transform)
        {
            // pos = �� child�� ��ǥ
            Vector3 pos = Playfield.instance.Round(child.position);
            Transform t = Playfield.instance.GetTransformOnGridPos(pos);

            // Ghost Block ���� ��ġ���ų� �ٸ� tetris block�� ��ġ�� �ʵ���
            if (t != null && t.parent == parent.transform)
            {
                return true;
            }

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