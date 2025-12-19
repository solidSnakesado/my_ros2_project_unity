using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BLOCK_COLOR
{
    PURPLE,
    GREEN,
    YELLOW
};

public class TargetBlock : MonoBehaviour
{
    public  BLOCK_COLOR     myColor     = BLOCK_COLOR.PURPLE;
    private ScenarioManager manager     = null;
    private bool            isFinished  = false;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<ScenarioManager>();
    }

    public void ResetBlockState()
    {
        isFinished          = false;
        Rigidbody rb        = GetComponent<Rigidbody>();
        rb.velocity         = Vector3.zero;
        rb.angularVelocity  = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isFinished == true)
        {
            return;
        }

        string hitTag = other.tag;
        bool isCorrect =
            (myColor == BLOCK_COLOR.GREEN && hitTag == "BoxGreen") ||
            (myColor == BLOCK_COLOR.PURPLE && hitTag == "BoxPurple") ||
            (myColor == BLOCK_COLOR.YELLOW && hitTag == "BoxYellow");

        if (isCorrect) 
        {
            isFinished = true;
            StartCoroutine(manager.OnTaskComplete());        
        }
    }
}
