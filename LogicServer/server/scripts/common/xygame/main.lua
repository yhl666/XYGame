
--[[
xygame 服务器 入口 lua脚本

xygame服务的核心部分，除了RPC服务这里都会提供
xygame服务器入口是 scripts.common.xygame.main

* Author: caoshanshan
* Email: me@dreamyouxi.com

开发前请看
common/base/remote.lua文件的说明
common/base/services_handler.lua文件的说明

]]


require("xygame.lib.init");




require("xygame.model.user");



print("xygame server init ok");

 