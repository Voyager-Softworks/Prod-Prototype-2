using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Expression : NetworkBehaviour
{
    public SkinnedMeshRenderer meshrenderer;
    public Material happyMat, defaultMat, sadMat, deadMat;
    
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
    [Command(requiresAuthority = false)]
    public void CmdSetExpression(ExpressionType expression)
    {
        RpcSetExpression(expression);
    }

    [ClientRpc]
    void RpcSetExpression(ExpressionType expression)
    {
        currentExpression = expression;
        switch(currentExpression)
        {
            case ExpressionType.Default:
                meshrenderer.materials[2].mainTexture = defaultMat.mainTexture;
                break;
            case ExpressionType.Happy:
                meshrenderer.materials[2].mainTexture = happyMat.mainTexture;
                break;
            case ExpressionType.Sad:
                meshrenderer.materials[2].mainTexture = sadMat.mainTexture;
                break;
            case ExpressionType.Dead:
                meshrenderer.materials[2].mainTexture = deadMat.mainTexture;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(meshrenderer == null || meshrenderer.enabled == false) return;
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
