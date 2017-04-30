/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com
using Behavior to peocess  AI not FSM
 */
using UnityEngine;
using System.Collections;

namespace BehaviorTree
{
    public enum NodeState
    {
        Running,//运行中
        Complete, // 完成
        Ready, // 待续
        Testing,//测试状态，行为节点执行后 返回上个条件节点继续检测 ，用于条件节点和行为节点之间的重复判定
    }
    public class NodeBase : Model
    {// s所有节点 基类
        public override void OnEnter()
        {

        }
        public override void OnExit()
        {

        }
        public override void UpdateMS()
        {

        }
        public override void Update()
        {

        }
        public bool RunAble()
        {
            return false;
        }

        public void AddChild(NodeBase node)
        {
            node.parent = this;
            children.Add(node);
        }
        public void RemoveChild(NodeBase node)
        {
            node.parent = null;
            children.Remove(node);
        }

        public virtual bool Visit(Entity target)
        {
            return false;
        }
        public bool IsLeaf = false;//叶子节点
        public NodeBase parent = null;//父节点
        public NodeState state = NodeState.Ready;
        protected ArrayList children = new ArrayList();

        //----------helper function

        public bool HasParent()
        {
            return parent != null;
        }


    }



    public class ActionBase : NodeBase
    {//行为节点基类

    }
    public class ConditionBase : NodeBase
    {//条件节点基类

    }






    //-------控制节点
    public class Selector : NodeBase
    {//选择节点
        public override bool Visit(Entity target)
        {//从子节点选择一个 执行
            foreach (NodeBase node in children)
            {
                if (node.Visit(target))
                {
                    return true;
                }
            }
            return false;
        }
    }
    public class SelectorRandom : NodeBase
    {//随机选择节点

    }
    public class Sequence : NodeBase
    {//序列节点
        public override bool Visit(Entity target)
        {//顺序执行 子节点  有一个返回false 即 终止 遍历 返回false，
            //所有子节点返回true 才返回true
            foreach (NodeBase node in children)
            {
                if (node.Visit(target)==false)
                {
                    return false;
                }
            }
            return true;
        }
    }
    public class Parallel : NodeBase
    {//并行节点


    }








    public class TreeController
    {//行为树控制器 负责整个树的 生命周期

    }











}