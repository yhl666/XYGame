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
            this.OnExit();
            if (OnComptele != null) OnComptele();
            GameObject.Destroy(this as Component);
        }
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
