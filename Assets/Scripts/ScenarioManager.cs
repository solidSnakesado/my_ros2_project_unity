using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioManager : MonoBehaviour
{
    public  GameObject  targetCube      = null;
    public  Collider    spawnArea       = null;
    public  Material[]  colorMateruals  = null;     // 0:Red, 1:Blue, 2:Yellow
    public  float       resetDelay      = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        SpawnRandomCube();
    }

    public void SpawnRandomCube()
    {
        TargetBlock block = targetCube.GetComponent<TargetBlock>();
        block.ResetBlockState();

        Bounds b = spawnArea.bounds;

        // x와 z 좌표를 영역 내에서 랜덤하게 생성합니다.
        // 기존 코드의 오류(b.max.y -> b.max.x)를 수정했습니다.
        float x = Random.Range(b.min.x, b.max.x);
        float z = Random.Range(b.min.z, b.max.z);

        targetCube.transform.position = new Vector3(x, b.max.y + 0.1f, z);

        int rand = Random.Range(0, colorMateruals.Length);
        targetCube.GetComponent<Renderer>().material = colorMateruals[rand];
        block.myColor = (BLOCK_COLOR)rand;
    }

    public IEnumerator OnTaskComplete()
    {
        yield return new WaitForSeconds(resetDelay);
        SpawnRandomCube();
    }
}
