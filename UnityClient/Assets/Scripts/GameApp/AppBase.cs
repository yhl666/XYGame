/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;


public class AppBase : GAObject
{
    public AppBase()
    {
        DATA.Init();
    }
    public WorldMap GetCurrentWorldMap()
    {
        return this.worldMap;
    }
    public virtual string GetAppName()
    {
        return "AppBase";
    }


    public void AddCellApp<T>() where T : CellApp, new() 
    {
        CellApp app = new T() as CellApp;
        app.parent = this;
        app.Init();
        this.Add(app);
    }

    public void RemoveCellApp(CellApp app)
    {
        this.Remove(app);
    }

    private void Add(CellApp b)
    {
        this.cells.Add(b);
        b.OnEnter();
    }
    private void Remove(CellApp b)
    {
        this.cells.Remove(b);
        b.OnExit();
        b.LazyDispose();
    }

    public override void UpdateMS()
    {
        base.UpdateMS();

        for (int i = 0; i<cells.Count;i++ )
        {
            if (this.IsInValid()) return;
            // if (b.IsValid()) 
            CellApp app = cells[i] as CellApp;
            { app.UpdateMS(); }
        }

        for (int i = 0; i < cells.Count; )
        {
            CellApp b = cells[i] as CellApp;
            if (b.IsInValid())
            {
                this.Remove(b);
            }
            else
            {
                ++i;
            }
        }

    }
    public override void OnDispose()
    {
        cells.Clear();
      ///  cells = null;
        base.OnDispose();
    }
    ArrayList cells = new ArrayList();

    protected WorldMap worldMap = null;
}
