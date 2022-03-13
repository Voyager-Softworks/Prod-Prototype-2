using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

//namespace NodeAI;
    [System.Serializable]
    public class LinkEvent : UnityEvent<Link> { };
    [System.Serializable]
    public class Link 
    {
        [SerializeField]
        public LinkPoint input;
        [SerializeField]
        public LinkPoint output;
        //public Action<Link> OnClick;
        [SerializeField]
        public LinkEvent OnClickEvent;

        public Link(LinkPoint input, LinkPoint output, LinkEvent OnClickEvent)
        {
            this.input = input;
            this.output = output;
            this.OnClickEvent = OnClickEvent;
        }
        

        public void Draw()
        {
            Handles.DrawBezier(
                input.rect.center,
                output.rect.center,
                input.rect.center - Vector2.left * 50f,
                output.rect.center + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            if(Handles.Button((input.rect.center + output.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
            {
                if(OnClickEvent != null)
                {
                    OnClickEvent.Invoke(this);
                }
            }
        }

        public void ProcessLink()
        {

        }

        
    }

