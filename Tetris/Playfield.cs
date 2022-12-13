using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
public class Playfield : MonoBehaviour 
{
    public static Playfield instance; // for TetrisBlcok.cs

    public int gridSizeX, gridSizeY, gridSizeZ;



    // ���� ����
    int layersCleared = 0;

    // �������� Ȯ���ϴ� bool ����
    // public bool isFinish;

    // Inspector ���� visual representation
    // �Ʒ� ����� �͵��� representation
    [Header("Tetris Blocks")]
    // �� blockList�� ����� ���� prefabs�� ��� drag -> �� 6 ������ tetris �����
    // spawn�� �� ���
    public GameObject[] blockList;
    // Ghost block���� ���� list -> blockList�� ���� ����
    public GameObject[] GhostList;




    [Header("Playfield Visuals")]
    public GameObject bottomPlane;
    public GameObject N, S, W, E;



    public Transform[,,] theGrid;
    
    // TetricBlock.cs�� ����
    void Awake()
    {
        instance = this; //
    }

    // INITIALIZE theGrid --> theGrid��� ���� ��ü�� �̿��ϱ� ����
    void Start()
    {
        theGrid = new Transform[gridSizeX, gridSizeY, gridSizeZ];
        SpawnNewBlock();

    }



    // TetricBlock.cs�� ����
    // position�� �ݿø� --> ��ǥ �񱳿� ����
    public Vector3 Round(Vector3 vec)
    {
        return new Vector3(Mathf.RoundToInt(vec.x),
                            Mathf.RoundToInt(vec.y),
                            Mathf.RoundToInt(vec.z));
    }

    // TetricBlock.cs�� ����
    // Block�� grid �ȿ� �ִ��� Ȯ���ϴ� �Լ�
    public bool CheckInsideGrid(Vector3 pos)
    {
        // grid ����  x, y, z ��ǥ ���� ����� �����Ͽ���.
        // �׷��Ƿ� block�� ��ǥ�� ��� ����̾�� -> is inside the grid
        // + gridsize���� ���� ��ǥ���̾�� -> is inside the grid
        // y ��ǥ�� ���Ѽ��� ����, 0���� ��ǥ���� ũ�⸸ �ϸ� �ȴ�.

        if ((int)pos.x >= 0 && (int)pos.x < gridSizeX
                && (int)pos.z >= 0 && (int)pos.z < gridSizeZ
                && (int)pos.y >= 0)
            return true;
        else
        {
            return false;
        }
        // return ((int)pos.x >= 0 && (int)pos.x < gridSizeX
           //     && (int)pos.z >= 0 && (int)pos.z < gridSizeZ
             //   && (int)pos.y >= 0);
    }

    public void UpdateGrid(TetrisBlock block)
    {
        // DELETE POSSIBLE PARENT OBJECTS -> parent: �ϳ��� block �� ��ü
        for( int x = 0; x < gridSizeX; x++)
        {
            for ( int z = 0; z<gridSizeZ; z++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    if (theGrid[x,y,z] != null)
                    {
                        // grid �ȿ� block�� ���� ���
                        if (theGrid[x, y, z].parent == block.transform)
                        {
                            theGrid[x, y, z] = null;
                        }
                    }

                    
                }
            }
        }

        // FILL IN THE CHILD OBJECTS -> child: �ϳ��� block�� �����ϴ� cubes
        foreach(Transform child in block.transform)
        {
            Vector3 pos = Round(child.position);
            // grid �ȿ� cube���� �����ϸ�
            if(pos.y < gridSizeY)
            {
                theGrid[(int)pos.x, (int)pos.y, (int)pos.z] = child;
            }

        }
    }

    // �ٸ� block�� �Ʒ��� �̹� �ִ� ��Ȳ�� ��
    // �� block�� ��ǥ ��������
    public Transform GetTransformOnGridPos(Vector3 pos)
    {
        // Grid �ۿ� ���� ��
        if(pos.y > gridSizeY - 1)
        {
            return null;
        }
        // Grid �ȿ� ���� ��
        else
        {

            return theGrid[(int)pos.x, (int)pos.y, (int)pos.z];
        }
    }


    public void SpawnNewBlock()
    {
        // Where to spawn
        // grid�� plane�� �������� (0,0,0)�� �������� 
        // ���� gridSizeX / 2, gridSizeY, gridSizeZ / 2�� �������Ƿ�
        // spawn ��Ҵ� grid�� ���� ���� ���߾��� ���̴�.
        Vector3 spawnPoint = new Vector3((int)transform.position.x + (float)gridSizeX / 2,
                                            (int)transform.position.y + gridSizeY+2,
                                            (int)transform.position.z + (float)gridSizeZ / 2);

        int randomIndex = Random.Range(0, blockList.Length); // 0~5 ������ ����

        // Spawn the tetris
        GameObject newBlock = Instantiate(blockList[randomIndex], spawnPoint, Quaternion.identity) as GameObject;
        // go to TetrisBlock.cs


        // Ghost
        GameObject newGhost = Instantiate(GhostList[randomIndex], spawnPoint, Quaternion.identity) as GameObject;   // �̷��� �Ǹ� tetris block�� ghost block�� spawn�� ���� ��ġ�� �����ȴ�.
        newGhost.GetComponent<ghostBlock>().SetParent(newBlock);


    }

    // called by TetrisBlock.cs
    public void DeleteLayer()
    {
        
        for (int y = gridSizeY-1; y >= 0; y--)
        {
            // Chenck full layer
            if (CheckFullLayer(y))
            {
                // �� ������� 1�� ����
                layersCleared += 1;

                // Delete all cubes in the layer
                DeleteLayerAt(y);


                // move all down by 1
                MoveAllLayerDown(y);
            }


        }
        
        if(layersCleared > 0)
        {
            // GameManager�� LayersCleared�Լ� ȣ��
            // GameManager.Instance.LayersCleared(layersCleared);
            UIHandler.instance.UpdateUI(layersCleared);
        }


        ///////////////// FINISH //////////////////
        if(layersCleared == 3 || layersCleared == 4)
        {
     
            SceneManager.LoadScene("Clear");

        }
        ///////////////////////////////////////////
    }

    bool CheckFullLayer(int y)
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                // Ư�� ������ grid�� �� ã���� Ȯ��
                if (theGrid[x,y,z] == null)
                {
                    return false;
                }
            }
        }

        return true;
    }


    void DeleteLayerAt(int y)
    {

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                // ��� cube ����
                Destroy(theGrid[x,y,z].gameObject);
                // �� layer �ʱ�ȭ
                theGrid[x, y, z] = null;

            }
        }
    }

    void MoveAllLayerDown(int y)
    {
        for (int i = y; i < gridSizeY; i++)
        {
            MoveOneLayerDown(i);
        }
    }
    
    void MoveOneLayerDown(int y)
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                if (theGrid[x,y,z] != null)
                {
                    // �� layer�� ���ٰ� ����
                    theGrid[x, y - 1, z] = theGrid[x, y, z];
                    // layer�� �������Ƿ� �� �� layer�� �ʱ�ȭ
                    theGrid[x, y, z] = null;
                    // �� layer�� ������ ����
                    theGrid[x, y-1, z].position += Vector3.down;

                }
            
            }
        }
    }


    void OnDrawGizmos()
    {
        // bottomPlane�� �����Ҷ�
        if(bottomPlane != null)
        {
            // Resize bottomPlane
            // scaler: rescale�� ����
            // y�� height�̹Ƿ� �ǵ��� �ʴ´�.
            Vector3 scaler = new Vector3((float)gridSizeX / 10, 1, (float)gridSizeZ / 10);
            bottomPlane.transform.localScale = scaler;  

            // Reposition
            bottomPlane.transform.position = new Vector3(transform.position.x + (float)gridSizeX/2
                                                            , transform.position.y
                                                            , transform.position.z + (float)gridSizeZ/2);


            // Retile material
            // material�� plane�� �Բ� �÷��ִ� �ڵ� --> plane�� �÷��� grid material ����� �̻������� �ʰ� �ϱ� ����
            bottomPlane.GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(gridSizeX, gridSizeZ);

        }

        // �Ȱ��� N, S, E, W�� ����
        // N, S, E, W�� �� �������� ������ grid ��
        // N�� S�� scale�� ���ص� z��ǥ�� ����
        // N�� �����Ҷ�
        if (N != null)
        {
            // Resize bottomPlane       
            Vector3 scaler = new Vector3((float)gridSizeX / 10, 1, (float)gridSizeY / 10);
            N.transform.localScale = scaler;

            // Reposition
            N.transform.position = new Vector3(transform.position.x + (float)gridSizeX / 2,
                                                            transform.position.y + (float)gridSizeY / 2,
                                                            transform.position.z + gridSizeZ);


            // Retile material
            N.GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(gridSizeX, gridSizeY);

        }

        // S�� �����Ҷ�
        if (S != null)
        {
            // Resize bottomPlane       
            Vector3 scaler = new Vector3((float)gridSizeX / 10, 1, (float)gridSizeY / 10);
            S.transform.localScale = scaler;

            // Reposition
            // gridSizeZ�� ������ �ʾƾ� S, ���ϸ� N
            S.transform.position = new Vector3(transform.position.x + (float)gridSizeX / 2,
                                                            transform.position.y + (float)gridSizeY / 2,
                                                            transform.position.z);


            // Retile material
            // S�� N�� ���� material�� retile��
            //S.GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(gridSizeX, gridSizeY);

        }

        // E�� W�� scale�� ���ص� x��ǥ�� ����
        // E�� �����Ҷ�
        if (E != null)
        {
            // Resize bottomPlane       
            Vector3 scaler = new Vector3((float)gridSizeZ / 10, 1, (float)gridSizeY / 10);
            E.transform.localScale = scaler;

            // Reposition
            E.transform.position = new Vector3(transform.position.x + gridSizeX,
                                                            transform.position.y + (float)gridSizeY / 2,
                                                            transform.position.z + (float)gridSizeZ / 2);


            // Retile material
            E.GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(gridSizeZ, gridSizeY);

        }

        // W�� �����Ҷ�
        if (W != null)
        {
            // Resize bottomPlane       
            Vector3 scaler = new Vector3((float)gridSizeZ / 10, 1, (float)gridSizeY / 10);
            W.transform.localScale = scaler;

            // Reposition
            // gridSizeX�� ������ �ʾƾ� W, ���ϸ� E
            W.transform.position = new Vector3(transform.position.x,
                                                            transform.position.y + (float)gridSizeY / 2,
                                                            transform.position.z + (float)gridSizeZ / 2);


            // Retile material
            // W�� E�� ���� material�� retile��
            // W.GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(gridSizeZ, gridSizeY);

        }

    }


}
