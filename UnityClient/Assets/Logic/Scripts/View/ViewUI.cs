using UnityEngine;
using System.Collections;
using UnityEngine.UI;



public class ViewImage : ViewUI
{
    public override bool Init()
    {
        base.Init();

        this.frame = SpriteFrameCache.ins.GetSpriteFrameAuto(png);/// SpriteFrame.CreateWithPng(png);//this.dao.png
        img.sprite = frame.sprite;

        return true;
    }

    public override void OnDispose()
    {
        frame = null;
      
        img = null;
    }


    public override void UpdateMS()
    {

    }


    public void InitWithDao(DAO.Equip dao)
    {

    }
    public void InitWithDao(DAO.Goods dao)
    {

    }


    private Image img = null;

    // inner
    private SpriteFrame frame = null;
    private string png = "hd/interface/items/2307.png";

    private DAO.DaoBase dao;
}

