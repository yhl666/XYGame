
--[[
* Author: caoshanshan
* Email: me@dreamyouxi.com

]]


local t = { }

local log = require("log"):new("worldchat")


function t.push(ctx, msg, cb)
    local kv = json.decode(msg);

    print(os.date("%Y-%m-%d-%H:%M", os.time()));

    local ymd = os.date("%Y-%m-%d", os.time());
    local hms = os.date("%H-%M-%S", os.time());
    room_notify_all("WorldChat", "Push", msg .. "time:" .. ymd .. ",time2:" .. hms .. ",");


end

 

return t;