using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Expression : NetworkBehaviour
{
    public SkinnedMeshRenderer meshrenderer;
    public Material happyMat, defaultMat, sadMat, deadMat;
    [SyncVar]
    public ExpressionType currentExpression;

    public enum ExpressionType
    {
        Default,
        Happy,
        Sad,
        Dead
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(currentExpression)
        {
            case ExpressionType.Default:
                meshrenderer.materials[2] = defaultMat;
                break;
            case ExpressionType.Happy:
                meshrenderer.materials[2] = happyMat;
                break;
            case ExpressionType.Sad:
                meshrenderer.materials[2] = sadMat;
                break;
            case ExpressionType.Dead:
                meshrenderer.materials[2] = deadMat;
                break;
        }
    }
}
