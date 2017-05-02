/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com
using Behavior Tree to peocess  AI not FSM
 */
using UnityEngine;
using System.Collections;

namespace BehaviorTree
{
    public enum ActionNodeState
    {//行为节点状态
        Running,//运行中，该状态下父节点会直接运行该节点逻辑， 
        Looping,//循环，因为行为节点都是条件导出， 该状态会 重新 执行该层级的 比如用于持续条件评定，
        Complete,//完成， 父节点可进入下个环节
        //  Failure,//执行失败，父节点进入下个环节
        UnKnown,//默认状态，什么都不知道
    }
    public enum NodeType
    {
        Condition,//条件节点
        Action, // 行为节点

        Selector,//选择节点   从子节点选择一个执行
        Sequence,//序列节点   从子节点依次执行 一般是条件 和动作的组合
        Parallel,//并行节点   执行所有节点

        UnKnown,
    }
    public class NodeBase// : Model
    {// s所有节点 基类
     /*   public override void OnEnter()
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

        }*/

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

        public virtual bool Visit(Entity target, ref ActionNodeState state)
        { // 行为节点需要携带 状态参数 返回给父类 
            return false;
        }
        public virtual bool Visit(Entity target)
        {//条件节点不需要 携带参数，
            return false;
        }
        //    public bool IsLeaf = false;//叶子节点
        public NodeBase parent = null;//父节点
        // public NodeState state = NodeState.Ready;
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
        public void Reset()
        {

        }
        public void BlockingTo(NodeBase node = null)
        {// 阻塞在目标node
            if(node == null)
            {//默认切换到root
                
            }
            // TODO

        }
        protected NodeType type = NodeType.UnKnown;

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
        private void ProcessActionNodeState(ActionNodeState state, NodeBase node)
        {
            if (state == ActionNodeState.UnKnown)
            {
                last_state = ActionNodeState.UnKnown;
                last_node = null;
            }
            else if (state == ActionNodeState.Looping)
            {
                last_state = ActionNodeState.Looping;
                last_node = this;
                this.BlockingTo(this);
            }
            else if (state == ActionNodeState.Running)
            {
                last_state = ActionNodeState.Running;
                last_node = node;
                this.BlockingTo(node);
            }
            else if (state == ActionNodeState.Complete)
            {
                last_state = ActionNodeState.UnKnown;
                last_node = null;
            }
            else
            {
                last_state = ActionNodeState.UnKnown;
                last_node = null;
            }
        }
        ActionNodeState last_state = ActionNodeState.UnKnown;
        NodeBase last_node = null;
        public override bool Visit(Entity target)
        {//从子节点选择一个 执行
            ActionNodeState state = ActionNodeState.UnKnown;
           /* if (last_state == ActionNodeState.Running && last_node!=null)
            { // 上次状态为
                last_node.Visit(target, ref state);
                this.ProcessActionNodeState(state, last_node);
                return true;
            }
            last_state = ActionNodeState.UnKnown;*/





            foreach (NodeBase node in children)
            {
                NodeType child_type = node.GetNodeType();
                bool ret = false;
                if (child_type == NodeType.Condition)
                {//子节点是条件节点， 条件评定
                    return false;// 理论上，选择节点中不存在 条件节点
                }
                else if (child_type == NodeType.Selector)
                {//子节点也是选择节点
                    ret = node.Visit(target);
                    if (ret)//成功表示 选择节点找到了目标不用继续遍历， 失败表示选择节点未选择到一个合适的 继续遍历
                    {
                        return true;
                    }
                }
                else if (child_type == NodeType.Action)
                {
                    ret = node.Visit(target, ref state);
                    this.ProcessActionNodeState(state, node);
                    if (ret)
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
        public override bool Visit(Entity target)
        {//顺序执行 子节点  有一个返回false 即 终止 遍历 返回false，
            //所有子节点返回true 才返回true
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
    {//并行节点


    }








    public class TreeController
    {//行为树控制器 负责整个树的 生命周期


        public NodeBase root;// 根
        public NodeBase current_node = null; // 当前执行的节点
    }
    public class Condition_HasHeroInAtkRange : ConditionBase
    {//仇恨范围内是否有英雄


    }
    public class Condition_HasTowerExists : ConditionBase
    {//是否存在塔

    }

    public class Condition_HasHitTower : ConditionBase
    {//是否第一次攻击到塔


    }
    public class Condition_HasHitByHero : ConditionBase
    {//是否被玩家攻击

    }

    public class Condition_HasTargetDie : ConditionBase
    {//目标是否死亡

    }






}