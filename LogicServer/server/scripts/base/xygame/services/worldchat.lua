
--[[
* Author: caoshanshan
* Email: me@dreamyouxi.com

]]


local t = { }

local log = require("log"):new("worldchat")


function t.push(ctx, msg, cb)
    local kv = json.decode(msg);

     room_notify_all ("WorldChat","Push",msg)

end

 

return t;