﻿采用帧同步
战斗帧同步服务器
战斗时 客户端与该服务器直连。
负责战斗的交互帧数据，暂时用多线程模型实现。等

可能采用的feature：
1.改用ASIO的协程模型替代现有的多线程模型。