#XYGame（造梦西游OL）  概述
#（------------------------------服务端--------------------------）
服务端按照实际需求分为3个部分

##战斗部分 BattleServer
![](http://git.oschina.net/dreamyouxi/XYGame/raw/master/%E8%AE%BE%E8%AE%A1%E6%96%87%E6%A1%A3/IMG_0741.jpg)
    采用帧同步
战斗帧同步服务器
战斗时 客户端与该服务器直连。
负责战斗的交互帧数据，暂时用多线程模型实现。

可能采用的feature：
1.改用ASIO的协程模型替代现有的多线程模型。

##非战斗部分 LogicServer
![](http://git.oschina.net/dreamyouxi/XYGame/raw/master/%E8%AE%BE%E8%AE%A1%E6%96%87%E6%A1%A3/IMG_0740.jpg)
    采用RPC回调完成逻辑
逻辑服务器采用应用层RPC来交互
只有脚本代码。runtime 为私有
拉取代码直接覆盖runtime的文件就能直接运行xygame服务器

#客户服务端 ClientServer
运行在服务端的软件。
用于模拟客户端的运算 来校验数据，原则上 只是UnityClient的一个场景 和部分修改代码
没有UI 只有逻辑，有单独的线程 负责和 LogicServer通信

#（------------------------客户端-----------------------）

#客户端 UnityClient
采用Unity实现可配置化实现C# 和Lua 热更新
功能：
![alt txt](https://git.oschina.net/dreamyouxi/Balls,"框架来自之前一个项目")