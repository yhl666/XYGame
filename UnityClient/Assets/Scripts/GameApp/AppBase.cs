using UnityEngine;
using System.Collections;


public class AppBase : GAObject
{
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

        foreach (CellApp app in cells)
        {
            // if (b.IsValid()) 

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
    ArrayList cells = new ArrayList();

    protected WorldMap worldMap = null;
}
