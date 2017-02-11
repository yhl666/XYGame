
--[[
µÇÂ¼·þÎñ
* Author: caoshanshan
* Email: me@dreamyouxi.com

]]



local t = { }

local log = require("log"):new("login")
local remote = require("xygame.base.remote");



function t.login(ctx, msg, cb)
    local kv = json.decode(msg);

    remote.request("services.redis", "getValue", "name:" .. kv["name"] .. ",", function(msg)

        local m = json.decode(msg);

        if kv["pwd"] == m["pwd"] then
            cb("res:ok,");
        else
            cb("res:error,");
        end

    end );


    remote.request_client(ctx, "rpctest4", "func", "name:3,", function(msg)
        print(msg .. "  recv respone");

    end );

end

function t.logout(msg, cb)

end

return t;