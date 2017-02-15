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
            GameObject.Destroy(this.GetComponent<Actions>());
        }
    }

    public float current_time = 0.0f;
    public float MAX_TIME = 1.0f;
    public VoidFuncVoid OnComptele = null;
}


public class ScaleTo : Actions
{
    public float start = 0.0f;
    public float end = 0.0f;
    public float current = 0.0f;

    public override void UpdateMS()
    {
        current = (end - start) * (current_time) / MAX_TIME + start;

        this.transform.localScale = (new Vector3(current, current, 1.0f));
    }

    public static Actions Create(GameObject target, float time, float to)
    {
        ScaleTo action = target.AddComponent<ScaleTo>();
        action.MAX_TIME = time;
        action.end = to;
        return action;
    }

    public override void OnExit()
    {
        //修正
        this.transform.localScale = (new Vector3(end, end, 1.0f));
    }
    public override void OnEnter()
    {
        start = this.transform.localScale.x;
    }

}




public class ScaleBy : ScaleTo
{
    public static Actions Create(GameObject target, float time, float by)
    {
        ScaleBy action = target.AddComponent<ScaleBy>();
        action.MAX_TIME = time;
        action.end = by;
        return action;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        end += start;
    }
}
