using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EventLogAnim : MonoBehaviour
{
    public AnimationClip openAnim;
    public AnimationClip closeAnim;

    public AnimationClip lastAnim;

    public float fadeTime = 1.0f;
    public float fadeDelay = 2.5f;
    private float fadeTimer = 0.0f;
    public bool keepOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        lastAnim = openAnim;
    }

    private void Update() {
        if (keepOpen) return;

        //count up the timer
        fadeTimer += Time.deltaTime;

        //fade image out over time
        Image elImage = GetComponent<Image>();
        if (elImage && fadeTimer >= fadeDelay && elImage.color.a > 0) {
            Color elColor = elImage.color;
            elColor.a -= Time.deltaTime / fadeTime;
            elImage.color = elColor;

            if (elColor.a <= 0) {
                Close();
            }
        }
    }

    public void KeepOpen(){
        keepOpen = true;
        Open();
    }

    public void Open()
    {
        if (lastAnim == openAnim) return;

        fadeTimer = 0.0f;

        //make oppacity full
        Image elImage = GetComponent<Image>();
        if (elImage) {
            Color elColor = elImage.color;
            elColor.a = 1;
            elImage.color = elColor;
        }

        GetComponent<Animation>().Play(openAnim.name);

        lastAnim = openAnim;
    }

    public void Release(){
        if (!keepOpen) return;

        keepOpen = false;
    }

    public void Close()
    {
        if (lastAnim == closeAnim) return;

        GetComponent<Animation>().Play(closeAnim.name);

        lastAnim = closeAnim;
    }
}
