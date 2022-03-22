using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBarrier : MonoBehaviour
{
    public enum BarrierType
    {
        GROUP_A,
        GROUP_B,
    }

    public BarrierType barrierType;
    public GameObject barrier;

    MeshRenderer meshRenderer;
    BoxCollider barrierCollider;

    bool initialized = false;

    public void SwitchBarriers(BarrierType type)
    {
        if(type == barrierType)
        {
            StartCoroutine(FadeInBarrier());
        }
        else
        {
            StartCoroutine(FadeOutBarrier());
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = barrier.GetComponent<MeshRenderer>();
        barrierCollider = barrier.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!initialized)
        {
            if(GameObject.FindGameObjectWithTag("SceneBarrierController"))
            {
                GameObject.FindGameObjectWithTag("SceneBarrierController").GetComponent<SceneBarrierController>().RegisterBarrier(this);
                initialized = true;
            }
        }
    }


    //Coroutine for fading in the barrier and enabling its collider
    public IEnumerator FadeInBarrier()
    {
        meshRenderer.enabled = true;
        barrierCollider.enabled = true;

        float elapsedTime = 0;
        float duration = 1;

        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            meshRenderer.material.SetFloat("_Opacity", t);
            yield return null;
        }
    }

    //Coroutine for fading out the barrier and disabling its collider
    public IEnumerator FadeOutBarrier()
    {
        float elapsedTime = 0;
        float duration = 1;

        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            meshRenderer.material.SetFloat("_Opacity", 1 - t);
            yield return null;
        }

        meshRenderer.enabled = false;
        barrierCollider.enabled = false;
    }
}
