/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

public class Actions : MonoBehaviour
{
    public virtual void UpdateMS()
    {

    }
    public virtual void OnExit()
    {

    }
    public virtual void OnEnter()
    {

    }

    void Start()
    {
        this.OnEnter();
    }
    void Update()
    {
        current_time += Time.deltaTime;
        this.UpdateMS();
        if (current_time > MAX_TIME)
        {
            this.Dispose();
        }
    }
    public void Dispose()
    {
        this.OnExit();
        if (OnComptele != null) OnComptele();
        GameObject.Destroy(this as Component);
    }
    public float current_time = 0.0f;
    public float MAX_TIME = 1.0f;
    public VoidFuncVoid OnComptele = null;
}


public class ScaleTo : Actions
{
    public float start_x = 0.0f;
    public float end_x = 0.0f;
    public float current_x = 0.0f;

    public float start_y = 0.0f;
    public float end_y = 0.0f;
    public float current_y = 0.0f;

    public override void UpdateMS()
    {
        current_x = (end_x - start_x) * (current_time) / MAX_TIME + start_x;
        current_y = (end_y - start_y) * (current_time) / MAX_TIME + start_y;

        this.transform.localScale = (new Vector3(current_x, current_y, 1.0f));
    }

    public static Actions Create(GameObject target, float time, float x, float y)
    {
        ScaleTo action = target.AddComponent<ScaleTo>();
        action.MAX_TIME = time;
        action.end_x = x;
        action.end_y = y;
        return action;
    }
    public static Actions Create(GameObject target, float time, float xy)
    {
        return Create(target, time, xy, xy);
    }
    public override void OnExit()
    {
        //修正
        this.transform.localScale = (new Vector3(end_x, end_y, 1.0f));
    }
    public override void OnEnter()
    {
        start_x = this.transform.localScale.x;
        start_y = this.transform.localScale.y;

    }

}


public class ScaleBy : ScaleTo
{
    public static Actions Create(GameObject target, float time, float x, float y)
    {
        ScaleBy action = target.AddComponent<ScaleBy>();
        action.MAX_TIME = time;
        action.end_x = x; action.end_y = y;
        return action;
    }
    public static Actions Create(GameObject target, float time, float xy)
    {
        return Create(target, time, xy, xy);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        end_x += start_x;
        end_y += start_y;

    }
}


public class FadeOut : Actions
{
    public float current_a = 1.0f;
    SpriteRenderer target = null;
    float speed = 0.0f;
    Color origned;
    public override void UpdateMS()
    {
        Color orign = target.color;
        current_a -= speed;
        target.color = new Color(orign.r, orign.g, orign.b, current_a);
    }

    public static Actions Create(SpriteRenderer target, float time)
    {
        FadeOut action = target.gameObject.AddComponent<FadeOut>();
        action.MAX_TIME = time;
        action.target = target;
        action.speed = time / 60.0f * target.color.a;
        return action;
    }
    public override void OnExit()
    {
        //修正
        target.color = new Color(origned.r, origned.g, origned.b, 0f);
    }
    public override void OnEnter()
    {
        origned = target.color;
    }

}

public class FadeIn : Actions
{
    public float current_a = 0.0f;
    SpriteRenderer target = null;
    UnityEngine.UI.Image target_img = null;
    float speed = 0.0f;
    Color origned;
    private Color GetColor()
    {
        if (target != null)
        {
            return target.color;
        }
        if (target_img != null)
        {
            return target_img.color;
        }
        return Color.white;
    }

    private void SetColor(Color c)
    {
        if (target != null)
        {
            target.color = c;
        }
        if (target_img != null)
        {
            target_img.color = c;
        }
    }
    public override void UpdateMS()
    {
        Color orign = GetColor();
        current_a += speed;
       SetColor( new Color(orign.r, orign.g, orign.b, current_a));
    }

    public static Actions Create(SpriteRenderer target, float time)
    {
        FadeIn action = target.gameObject.AddComponent<FadeIn>();
        action.MAX_TIME = time;
        action.target = target;
        action.speed = time / 60.0f;
        return action;
    }

    public static Actions Create(GameObject target, float time)
    {
        FadeIn action = target.gameObject.AddComponent<FadeIn>();
        action.MAX_TIME = time;
        action.target = target.GetComponent<SpriteRenderer>();
        action.target_img = target.GetComponent<UnityEngine.UI.Image>();
        action.speed = time / 60.0f;
        return action;
    }
    public override void OnExit()
    {
        //修正
        SetColor(new Color(origned.r, origned.g, origned.b, 1.0f));
    }
    public override void OnEnter()
    {
        origned = GetColor();
    }

}


public class MoveTo : Actions
{//code copy from ScaleTo
    public float start_x = 0.0f;
    public float end_x = 0.0f;
    public float current_x = 0.0f;

    public float start_y = 0.0f;
    public float end_y = 0.0f;
    public float current_y = 0.0f;

    public override void UpdateMS()
    {
        current_x = (end_x - start_x) * (current_time) / MAX_TIME + start_x;
        current_y = (end_y - start_y) * (current_time) / MAX_TIME + start_y;

        this.transform.localPosition = (new Vector3(current_x, current_y, this.transform.localPosition.z));
    }

    public static Actions Create(GameObject target, float time, float x, float y)
    {
        MoveTo action = target.AddComponent<MoveTo>();
        action.MAX_TIME = time;
        action.end_x = x;
        action.end_y = y;
        return action;
    }
    public static Actions Create(GameObject target, float time, float xy)
    {
        return Create(target, time, xy, xy);
    }
    public override void OnExit()
    {
        //修正
        this.transform.localPosition = (new Vector3(end_x, end_y, this.transform.localPosition.z));
    }
    public override void OnEnter()
    {
        start_x = this.transform.localPosition.x;
        start_y = this.transform.localPosition.y;

    }
}

public class MoveBy : MoveTo
{
    public static Actions Create(GameObject target, float time, float x, float y)
    {
        MoveBy action = target.AddComponent<MoveBy>();
        action.MAX_TIME = time;
        action.end_x = x; action.end_y = y;
        return action;
    }
    public static Actions Create(GameObject target, float time, float xy)
    {
        return Create(target, time, xy, xy);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        end_x += start_x;
        end_y += start_y;

    }
}


public class CallFunc : Actions
{
    private VoidFuncVoid cb = null;

    public static Actions Create(GameObject target, float time, VoidFuncVoid cb = null)
    {
        CallFunc action = target.AddComponent<CallFunc>();
        action.MAX_TIME = time;
        action.cb = cb;
        return action;
    }

    public override void OnExit()
    {
        if (cb != null)
            cb();
    }
}

