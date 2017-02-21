运行在服务端的软件。
用于模拟客户端的运算 来校验数据，原则上 只是UnityClient的一个场景 和部分修改代码
没有UI 只有逻辑，有单独的线程 负责和 LogicServer通信
运算的输入来自BattleServer，暂时用文件来持久化，不用Redis，因为一场PVP战斗帧数据大概为2MB，而且
不用对UnityClient和BattleServer添加Redis支持