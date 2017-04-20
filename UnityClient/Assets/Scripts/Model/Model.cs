/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

public class Model : GAObject
{
    public override void Update()
    {

    }
    public override void UpdateMS()
    {

    }
    public override bool Init()
    {

        return true;
    }
    public Model() { }
    public bool visible = true;
    ///  public View view = null;

}

public sealed class DirInput : Model
{
    public int dir
    {
        set
        {
            this._dir = value;
        }
        get
        {
            return this._dir;
        }
    }
    private int _value = 1;
    private float _x = 0.0f;
    private float _y = 0.0f;
    private int _dir = -1;

    public float rotateZ_arrow;
    public float x_center;
    public float y_center;

    public float x_arrow;
    public float y_arrow;

    public float x_bg;
    public float y_bg;

    public Vector2 pos_began;


    public override void UpdateMS()
    {
        base.UpdateMS();
    }
    public override bool Init()
    {
        base.Init();
        ins = this;
        ///   ModelMgr.ins.Add(this as Model);

        EventDispatcher.ins.AddEventListener(this, Events.ID_UI_WAIT);
        EventDispatcher.ins.AddEventListener(this, Events.ID_UI_NOWAIT);
        SetPosition(new Vector2(Screen.width / 8, Screen.height / 7));
        return true;
    }

    public static DirInput ins = null;

    bool isTouch = false;

    // private int dir = -1;
    private int Degree = -1;

    public bool isLight;

    public override void OnEvent(int type, object userData)
    {
        if (type == Events.ID_ABLE_DIRINPUT)
        {
            this.enable = true;
        }
        else if (type == Events.ID_DISABLE_DIRINPUT)
        {
            this.enable = false;
        }

        switch (type)
        {
            case Events.ID_UI_WAIT:
                {
                    this.enable = true;

                } break;
            case Events.ID_UI_NOWAIT:
                {
                    this.enable = false;
                } break;
        }

    }

    private bool enable = false;

    void OnTouchBegan(float x, float y)
    {
     ///   this.SetPosition(pos_began);
    }

    void SetPosition(Vector2 pos)
    {
        //if (float.IsNaN(pos.x) == false && float.IsNaN(pos.y) == false)
        {
            Vector2 v = pos;// ConvertToUIVector2(pos);
            x_center = v.x;
            y_center = v.y;

            x_arrow = 0.0f;//this.x;
            y_arrow = 0.0f; //this.y;

            x_bg = v.x;
            y_bg = v.y;


            /* this.x = v.x;
            this.y = v.y;

            x_center = this.x;
            y_center = this.y;

            x_arrow = 0.0f;//this.x;
            y_arrow = 0.0f; //this.y;

            x_bg = x;
            y_bg = y;*/
        }
    }

    void OnTouchEnded(float x, float y)
    {
        this.move=false;
        this._enable = false;
        //  dir = -1;
        //    SetPosition(pos_began);
        SetPosition(new Vector2(Screen.width / 8, Screen.height / 6));
        PublicData.ins.IS_dir = -1;
    }

    private Vector2 ConvertToUIVector2(Vector2 pos)
    {
        //  pos.x -= 1136f / 2.0f;// Screen.width / 2.0f;
        //  pos.y -= 640f / 2.0f;// Screen.height / 2.0f;
        return pos;
    }
    void OnTouchMoved(float x, float y)
    {
        move = true;
        this._enable = true;
        this.SetPosition(pos_began);
        //   Debug.Log("mov");
        //8方向 0 1 2 3 4 5 6 7

        float factor = (Screen.height / 640f);
        float DELTA_XY = 80.0f * factor;//图片移动范围
        const float ONE_DEGREE = DATA.ONE_DEGREE;//一度的弧度

        float dx = x - (pos_began.x);
        float dy = y - (pos_began.y);
        float dposx = 0.0f, dposy = 0.0f;

        int dir = this.dir;//方向
        float degree = Mathf.Atan(dy / dx) * 57.29578f;

        if (float.IsNaN(degree)) return;
        float degree_total = 0.0f;

        if (dx == 0.0f || dy == 0.0f)
        {
            //return;
        }

        float half_half = 45.0f / 2.0f;

        if (dx >= 0.0f && dy >= 0.0f)// 0 1 2 //1
        {
            if (degree < half_half) dir = 0;
            else if (degree > half_half + 45.0f) dir = 2;
            else dir = 1;
            dposy = DELTA_XY * Mathf.Sin(degree * ONE_DEGREE);
            dposx = DELTA_XY * Mathf.Cos(degree * ONE_DEGREE);
            degree_total = degree;
        }
        else if (dx >= 0.0f && dy <= 0.0f)// 0 7 6 //4
        {
            degree = -degree;
            if (degree < half_half) dir = 0;
            else if (degree > half_half + 45.0f) dir = 6;
            else dir = 7;
            dposy = -DELTA_XY * Mathf.Sin(degree * ONE_DEGREE);
            dposx = DELTA_XY * Mathf.Cos(degree * ONE_DEGREE);

            degree_total = 360.0f - degree;
        }
        else if (dx <= 0.0f && dy >= 0.0f)// 2 3 4 //2
        {
            degree = -degree;
            if (degree < half_half) dir = 4;
            else if (degree > 45.0f + half_half) dir = 2;
            else dir = 3;
            dposy = DELTA_XY * Mathf.Sin(degree * ONE_DEGREE);
            dposx = -DELTA_XY * Mathf.Cos(degree * ONE_DEGREE);
            degree_total = 180.0f - degree;
        }
        else//4 5 6 // 3
        {
            if (degree < half_half) dir = 4;
            else if (degree > 45 + half_half) dir = 6;
            else dir = 5;
            dposy = -DELTA_XY * Mathf.Sin(degree * ONE_DEGREE);
            dposx = -DELTA_XY * Mathf.Cos(degree * ONE_DEGREE);
            // degree_total = 180 + degree;
            degree_total = 180.0f + degree;
        }


        if (float.IsNaN(dposx) == false || float.IsNaN(dposy) == false)
        {
            Vector2 v = ConvertToUIVector2(new Vector2(pos_began.x + dposx, pos_began.y + dposy));
            this.x_center = v.x;
            this.y_center = v.y;
            /// 8 direction
            //   float[] pos_x = { 42.0f, 37.0f, 0.0f, -37.0f, -42f, -37.0f, 0.0f, 37.0f };
            //   float[] pos_y = { 0.0f, 37.0f, 42.0f, 37.0f, 0.0f, -37.0f, -42f, -37.0f };

            y_arrow = 110 * factor * Mathf.Sin(degree_total * ONE_DEGREE);// *yy[d / 90];
            x_arrow = 110 * factor * Mathf.Cos(degree_total * ONE_DEGREE);// *xx[d / 90];

            rotateZ_arrow = degree_total - 270.0f;
            this.dir = dir;
            this.Degree = (int)degree_total;
            PublicData.ins.IS_dir = this.Degree;
        }
        else
        {
            y_arrow = 0.0f;
            x_arrow = 0.0f;
            rotateZ_arrow = 0.0f;
            Debug.Log("invalid  value");

        }
    }
    public override void Update()
    {
        if (enable) return;
        if (HeroMgr.ins.self != null && HeroMgr.ins.self.isDie)
        {
            _enable = false;
            return;
        }
        /*   Ball self = BallsMgr.ins.GetSelfBalls();
           if(self.alive)
           {

           }
           else
           {
               this._enable = false;
               return;
           }*/

        do
        {
            if (Input.GetButton("Fire1") && isTouch == false && move ==false)
            {
                float x = GetCurrentMousePositionX(), y = GetCurrentMousePositionY();
                if (x < Screen.width / 2)
                {//屏幕左边才生效
                    // pos_began.y =( Input.mousePosition.y+42) / Screen.height * 10.0f;
                    //   pos_began.x = (Input.mousePosition.x+43 )/ Screen.width * 21.0f;

                    //   pos_began = new Vector3(x - Screen.width / 2.0f, y - Screen.height / 2.0f, 0.0f);

                    pos_began = new Vector2(x, y);

                    isTouch = true;
                    this.OnTouchBegan(x, y);
                }
            }
            if (Input.GetButton("Fire1") == false && isTouch == true )
            {
                if (isTouch == true)
                {
                    this.OnTouchEnded(GetCurrentMousePositionX(), GetCurrentMousePositionY());

                    isTouch = false;
                }
            }

            if (isTouch == true)
            {
                if (GetCurrentMousePositionX() > Screen.width / 2) break;
                if (GetCurrentMousePositionX() != pos_began.x && GetCurrentMousePositionY() != pos_began.y)
                {
                    this.OnTouchMoved(GetCurrentMousePositionX(), GetCurrentMousePositionY());
                }
            }

        } while (false);

    }
    float GetCurrentMousePositionX()
    {
        return Input.mousePosition.x;///(Screen.width / 1136f) / 2f;
    }
    float GetCurrentMousePositionY()
    {
        return Input.mousePosition.y;/// /(Screen.height / 640f)/2f;
    }
    public bool _enable = false;
    private bool move=false;
}