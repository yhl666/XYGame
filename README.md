#XYGame（造梦西游OL） 概述
(申明:本游戏图片资源来自4399原版,仅用于学习使用,如有侵权请联系我删除 me@dreamyouxi.com)。
#相比原游戏添加 多人PVP  PVE  等大型功能


#----------------------------------------------服务端概述------------------------------------------
服务端按照实际需求分为3个部分

##1.战斗部分 BattleServer
![](http://git.oschina.net/dreamyouxi/XYGame/raw/master/%E8%AE%BE%E8%AE%A1%E6%96%87%E6%A1%A3/IMG_0741.jpg)
    采用帧同步
战斗帧同步服务器
战斗时 客户端与该服务器直连。
负责战斗的交互帧数据，暂时用多线程模型实现。等
[基础框架来自之前一个项目](https://git.oschina.net/dreamyouxi/Balls)
可能采用的feature：
1.改用ASIO的协程模型替代现有的多线程模型。

##2.非战斗部分 LogicServer
![](http://git.oschina.net/dreamyouxi/XYGame/raw/master/%E8%AE%BE%E8%AE%A1%E6%96%87%E6%A1%A3/IMG_0740.jpg)
使用C++ 和Lua 实现RPC回调完成逻辑
逻辑服务器采用应用层RPC来交互
只有脚本代码。runtime 为私有
拉取代码直接覆盖runtime的文件就能直接运行xygame服务器
内部对原runtime进行了扩展，比如Lua层实现"真正的RPC"开发方法
对ClientServer交互的扩展支持等
添加对Redis的支持，未来可能会 添加 MySql持久化 来实现热冷数据 概念

##3.客户服务端 ClientServer
运行在服务端的软件。
用于模拟客户端的运算 来校验数据，原则上 只是UnityClient的一个场景 和部分修改代码
没有UI 只有逻辑，有单独的线程 负责和 LogicServer通信等

#------------------------------------------------客户端概述--------------------------------------

#客户端 UnityClient
采用Unity实现可配置化C# 和Lua ，动态切换Lua 和C#代码 达到热更新目的
基础功能：
[基础框架来自之前一个项目（Balls-小球大作战）](https://git.oschina.net/dreamyouxi/Balls)
在此基础上的改进和扩展:
1.添加小程序概念（类似于微信小程序的沙盒机制），详见代码中各种XXXApp.cs
2.添加RPC调用
3.添加骨骼动画(来自Spine官方Unity库的简单封装)
4.添加对来自cocos2dx 的帧动画 plist png等文件的处理（包括像素切图详见2D文件夹），实现一行代码播放帧动画
5.添加对2D动画的支持，比如ScaleTo动画等，详见游戏中各种UI的动画
6.添加对Lua实现部分逻辑的支持详见LuaBuffer 还是可以用可配置化的逻辑比如BulletConfig.cs 
7.实现战斗中的混合状态机详见StateMachine.cs和StateBase.cs
8.添加对战斗地图地形的可视化编辑达到输入输出 详见 Terrain.cs 和 WorldMap.cs
9.等等

#-------------------------------------------------已实现的玩法---------------------------
1.好友系统，日常好友管理（添加，PVP，删除，等）
2.世界聊天系统，
3.战斗系统 包括PVE PVP 部分实现