
--[[

* Author: caoshanshan
* Email: me@dreamyouxi.com

]]


local t = { }

local log = require("log"):new("login")
local remote = require("xygame.base.remote");-- 远程服务
-- 和客户端交互的服务实例

-- [Comment]
-- ctx 上下文，因为可能要发起客户端调用所以需要带上ctx
-- msg #string 数据 是简单的json格式全为字符串
-- cb #function 需要响应的时候的回调函数，参数为简单json格式
function t.login(ctx, msg, cb)
    local kv = json.decode(msg);

    -- 发起cell服务器的远程调用，请求 数据库服务
    remote.request("services.example", "getValue", "name:" .. kv["name"] .. ",", function(msg)

        local m = json.decode(msg);

        if kv["pwd"] == m["pwd"] then
            cb("res:ok,");
        else
            cb("res:error,");
        end

    end );

    -- 向游戏客户端发起rpc调用
    remote.request_client(ctx, "SampleServices", "method", "name:3,", function(msg)
        print(msg .. "  recv respone");

    end );

end

function t.logout(msg, cb)

end

return t;


