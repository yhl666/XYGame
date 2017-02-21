
--[[

* Author:  sutao
* Email:   sutao@ztgame.com


--{no:1,no_target:2,}no我的id no_target 我要战的id
--request_pvp_v1(ctx, msg, cb)

end
]]


local t = { }

local log = require("log"):new("login")
local remote = require("base.remote");
local hero = require("model.base_hero")
--[Common]
--{no:1,no_target:2,}
function t.request_pvp_v1(ctx, msg, cb)

    remote.request("services.battle_pvp", "request_pvp_v1", msg, function(msg1)
        if msg == "" then
            cb("ret:error,msg:timeout,"); 
            return;
        end 
        cb(msg1);
    end);
end


return t;