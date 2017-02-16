
--[[
初始化一些模块等
* Author: caoshanshan
* Email: me@dreamyouxi.com

]]


require("xygame.lib.extern");
require("xygame.lib.json");
require("xygame.lib.common")
require("xygame.lib.json")


package.path = package.path .. ";" .. G_LUA_ROOTPATH .. "/" .. "common/xygame/" .. "/?.lua"
package.path = package.path .. ";" .. G_LUA_ROOTPATH .. "/" .. "cell/xygame/" .. "/?.lua"
package.path = package.path .. ";" .. G_LUA_ROOTPATH .. "/" .. "base/xygame/" .. "/?.lua"
 

print("lib init ok");