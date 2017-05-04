/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com
using Behavior Tree to peocess  AI
 * 行为树框架
 */
using UnityEngine;
using System.Collections;

namespace BehaviorTree
{
    //----------------行为树框架部分
    /* public enum ActionNodeState
     {//行为节点状态
         Running,//运行中，该状态下父节点会直接运行该节点逻辑， 
         Looping,//循环，因为行为节点都是条件导出， 该状态会 重新 执行该层级的 比如用于持续条件评定，
         Complete,//完成， 父节点可进入下个环节
         //  Failure,//执行失败，父节点进入下个环节
         UnKnown,//默认状态，什么都不知道
     }*/
    public enum NodeType
    {
        Condition,//条件节点
        Action, // 行为节点

        Selector,//选择节点   从子节点选择一个执行
        Sequence,//序列节点   从子节点依次执行 一般是条件 和动作的组合
        Parallel,//并行节点   执行所有节点

        UnKnown,
    }
    public class NodeBase : Model // Model for 事件系统 和 生命周期管理协议
    {// 所有节点 基类
        public override void OnEnter()
        {

        }
        public override void OnExit()
        {

        }
        public override void OnEvent(int type, object userData)
        {

        }
        public sealed override void UpdateMS() { }
        public sealed override void Update() { }

        public void AddChild(NodeBase node)
        {
            if (IsConflict(node))
            {
                return;
            }
            node.parent = this;
            children.Add(node);
        }
        public void RemoveChild(NodeBase node)
        {
            node.parent = null;
            children.Remove(node);
        }
        public virtual bool Visit(Entity target)
        {//条件节点不需要 携带参数，
            return false;
        }
        public NodeBase parent = null;//父节点
        protected ArrayList children = new ArrayList();

        //----------helper function
        public bool IsConflict(NodeBase other)
        {// 节点间 是否冲突
            // 比如 选择节点的子节点中 不应该有条件节点
            return false;
        }
        public bool HasParent()
        {
            return parent != null;
        }
        public NodeType GetNodeType()
        {
            return type;
        }
        protected NodeType type = NodeType.UnKnown;
        protected Entity _host = null;
    }

    public class ActionBase : NodeBase
    {//行为节点基类
        public ActionBase()
        {
            this.type = NodeType.Action;
        }
    }
    public class ConditionBase : NodeBase
    {//条件节点基类
        public ConditionBase()
        {
            this.type = NodeType.Condition;
        }
    }

    //-----------------------------------------------------------------控制节点
    public class ControllBase : NodeBase
    {

    }
    public class Selector : ControllBase
    {//选择节点
        public Selector()
        {
            this.type = NodeType.Selector;
        }
        public override bool Visit(Entity target)
        {//从子节点选择一个 执行
            foreach (NodeBase node in children)
            {
                NodeType child_type = node.GetNodeType();
                if (child_type == NodeType.Condition)
                {//子节点是条件节点， 条件评定
                    return false;// 选择节点中不应该存在 条件节点
                }
                else
                {//选择一个节点即可
                    if (node.Visit(target))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
    public class Sequence : ControllBase
    {//序列节点
        public Sequence()
        {
            this.type = NodeType.Sequence;
        }
        public override bool Visit(Entity target)
        {//一个返回false 即 返回false
            foreach (NodeBase node in children)
            {
                if (node.Visit(target) == false)
                {
                    return false;
                }
            }
            return true;
        }
    }
    public class Parallel : ControllBase
    {//并行节点 所有节点都返回true 才返回true 
        public Parallel()
        {
            this.type = NodeType.Parallel;
        }
        public override bool Visit(Entity target)
        {//从子节点选择一个 执行 都返回false 才返回false 否则返回true
            if (children.Count <= 0) return false;
            bool ret = true;
            foreach (NodeBase node in children)
            {
                if (node.Visit(target) == false)
                {
                    ret = false;
                }
            }
            return ret;
        }
    }
}