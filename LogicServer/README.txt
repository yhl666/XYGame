使用C++ 和Lua 实现RPC回调完成逻辑
逻辑服务器采用应用层RPC来交互
只有脚本代码。runtime 为私有
拉取代码直接覆盖runtime的文件就能直接运行xygame服务器
内部对原runtime进行了扩展，比如Lua层实现"真正的RPC"开发方法
对ClientServer交互的扩展支持等
添加对Redis的支持，未来可能会 添加 MySql持久化 来实现热冷数据 概念