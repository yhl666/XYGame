using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;



[AddComponentMenu("Extensions/GAButton")]
public sealed class GAButton : Selectable, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler
{
    public VoidFuncVoid onEnter = null;
    public VoidFuncVoid onExit = null;
    public VoidFuncVoid onOver = null;
    private Image img = null;

    bool isEnter = false;

    protected override void Start()
    {
        this.img = this.GetComponent<Image>();
    }
    void Update()
    {
        if (isEnter)
        {
            if (onOver != null) onOver();

        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        //  Debug.Log("down");

    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        Debug.Log("enter");
        if (onEnter != null) onEnter();
        this.img.color = new Color32(193, 193, 193, 255);
        isEnter = true;


    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        Debug.Log("exit");
        if (onExit != null) onExit();
        this.img.color = new Color32(255, 255, 255, 255);
        isEnter = false;

    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        //   Debug.Log("up");

    }

}
