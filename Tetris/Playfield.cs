using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
public class Playfield : MonoBehaviour 
{
    public static Playfield instance; // for TetrisBlcok.cs

    public int gridSizeX, gridSizeY, gridSizeZ;



    // 점수 변수
    int layersCleared = 0;

    // 끝났는지 확인하는 bool 변수
    // public bool isFinish;

    // Inspector 안의 visual representation
    // 아래 선언된 것들의 representation
    [Header("Tetris Blocks")]
    // 이 blockList에 만들어 놓은 prefabs를 모두 drag -> 총 6 종류의 tetris 저장됨
    // spawn할 때 사용
    public GameObject[] blockList;
    // Ghost block들을 위한 list -> blockList와 같은 역할
    public GameObject[] GhostList;




    [Header("Playfield Visuals")]
    public GameObject bottomPlane;
    public GameObject N, S, W, E;



    public Transform[,,] theGrid;
    
    // TetricBlock.cs에 쓰임
    void Awake()
    {
        instance = this; //
    }

    // INITIALIZE theGrid --> theGrid라는 변수 자체를 이용하기 위해
    void Start()
    {
        theGrid = new Transform[gridSizeX, gridSizeY, gridSizeZ];
        SpawnNewBlock();

    }



    // TetricBlock.cs에 쓰임
    // position을 반올림 --> 좌표 비교에 용이
    public Vector3 Round(Vector3 vec)
    {
        return new Vector3(Mathf.RoundToInt(vec.x),
                            Mathf.RoundToInt(vec.y),
                            Mathf.RoundToInt(vec.z));
    }

    // TetricBlock.cs에 쓰임
    // Block이 grid 안에 있는지 확인하는 함수
    public bool CheckInsideGrid(Vector3 pos)
    {
        // grid 어디든  x, y, z 좌표 값이 양수로 제작하였다.
        // 그러므로 block의 좌표가 모두 양수이어야 -> is inside the grid
        // + gridsize보다 작은 좌표값이어야 -> is inside the grid
        // y 좌표는 상한선이 없고, 0보다 좌표값이 크기만 하면 된다.

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
        // DELETE POSSIBLE PARENT OBJECTS -> parent: 하나의 block 그 자체
        for( int x = 0; x < gridSizeX; x++)
        {
            for ( int z = 0; z<gridSizeZ; z++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    if (theGrid[x,y,z] != null)
                    {
                        // grid 안에 block이 없는 경우
                        if (theGrid[x, y, z].parent == block.transform)
                        {
                            theGrid[x, y, z] = null;
                        }
                    }

                    
                }
            }
        }

        // FILL IN THE CHILD OBJECTS -> child: 하나의 block을 구성하는 cubes
        foreach(Transform child in block.transform)
        {
            Vector3 pos = Round(child.position);
            // grid 안에 cube들이 존재하면
            if(pos.y < gridSizeY)
            {
                theGrid[(int)pos.x, (int)pos.y, (int)pos.z] = child;
            }

        }
    }

    // 다른 block이 아래에 이미 있는 상황일 때
    // 그 block의 좌표 가져오기
    public Transform GetTransformOnGridPos(Vector3 pos)
    {
        // Grid 밖에 있을 때
        if(pos.y > gridSizeY - 1)
        {
            return null;
        }
        // Grid 안에 있을 때
        else
        {

            return theGrid[(int)pos.x, (int)pos.y, (int)pos.z];
        }
    }


    public void SpawnNewBlock()
    {
        // Where to spawn
        // grid의 plane의 기준점인 (0,0,0)을 기준으로 
        // 각각 gridSizeX / 2, gridSizeY, gridSizeZ / 2을 더했으므로
        // spawn 장소는 grid의 가장 위의 정중앙일 것이다.
        Vector3 spawnPoint = new Vector3((int)transform.position.x + (float)gridSizeX / 2,
                                            (int)transform.position.y + gridSizeY+2,
                                            (int)transform.position.z + (float)gridSizeZ / 2);

        int randomIndex = Random.Range(0, blockList.Length); // 0~5 사이의 숫자

        // Spawn the tetris
        GameObject newBlock = Instantiate(blockList[randomIndex], spawnPoint, Quaternion.identity) as GameObject;
        // go to TetrisBlock.cs


        // Ghost
        GameObject newGhost = Instantiate(GhostList[randomIndex], spawnPoint, Quaternion.identity) as GameObject;   // 이렇게 되면 tetris block과 ghost block이 spawn시 같은 위치에 생성된다.
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
                // 층 사라지면 1씩 증가
                layersCleared += 1;

                // Delete all cubes in the layer
                DeleteLayerAt(y);


                // move all down by 1
                MoveAllLayerDown(y);
            }


        }
        
        if(layersCleared > 0)
        {
            // GameManager의 LayersCleared함수 호출
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
                // 특정 층에서 grid가 꽉 찾는지 확인
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
                // 모든 cube 삭제
                Destroy(theGrid[x,y,z].gameObject);
                // 그 layer 초기화
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
                    // 윗 layer와 같다고 정의
                    theGrid[x, y - 1, z] = theGrid[x, y, z];
                    // layer를 내렸으므로 그 윗 layer는 초기화
                    theGrid[x, y, z] = null;
                    // 한 layer를 실제로 내림
                    theGrid[x, y-1, z].position += Vector3.down;

                }
            
            }
        }
    }


    void OnDrawGizmos()
    {
        // bottomPlane이 존재할때
        if(bottomPlane != null)
        {
            // Resize bottomPlane
            // scaler: rescale의 기준
            // y는 height이므로 건들지 않는다.
            Vector3 scaler = new Vector3((float)gridSizeX / 10, 1, (float)gridSizeZ / 10);
            bottomPlane.transform.localScale = scaler;  

            // Reposition
            bottomPlane.transform.position = new Vector3(transform.position.x + (float)gridSizeX/2
                                                            , transform.position.y
                                                            , transform.position.z + (float)gridSizeZ/2);


            // Retile material
            // material도 plane과 함께 늘려주는 코드 --> plane을 늘려도 grid material 모양이 이상해지지 않게 하기 위해
            bottomPlane.GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(gridSizeX, gridSizeZ);

        }

        // 똑같이 N, S, E, W에 적용
        // N, S, E, W는 각 동서남북 방향의 grid 벽
        // N과 S는 scale이 변해도 z좌표가 고정
        // N이 존재할때
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

        // S가 존재할때
        if (S != null)
        {
            // Resize bottomPlane       
            Vector3 scaler = new Vector3((float)gridSizeX / 10, 1, (float)gridSizeY / 10);
            S.transform.localScale = scaler;

            // Reposition
            // gridSizeZ를 더하지 않아야 S, 더하면 N
            S.transform.position = new Vector3(transform.position.x + (float)gridSizeX / 2,
                                                            transform.position.y + (float)gridSizeY / 2,
                                                            transform.position.z);


            // Retile material
            // S는 N에 의해 material이 retile됨
            //S.GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(gridSizeX, gridSizeY);

        }

        // E과 W는 scale이 변해도 x좌표가 고정
        // E가 존재할때
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

        // W가 존재할때
        if (W != null)
        {
            // Resize bottomPlane       
            Vector3 scaler = new Vector3((float)gridSizeZ / 10, 1, (float)gridSizeY / 10);
            W.transform.localScale = scaler;

            // Reposition
            // gridSizeX를 더하지 않아야 W, 더하면 E
            W.transform.position = new Vector3(transform.position.x,
                                                            transform.position.y + (float)gridSizeY / 2,
                                                            transform.position.z + (float)gridSizeZ / 2);


            // Retile material
            // W는 E에 의해 material이 retile됨
            // W.GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(gridSizeZ, gridSizeY);

        }

    }


}
