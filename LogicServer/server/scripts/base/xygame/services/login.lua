
--[[
登录服务
* Author: caoshanshan
* Email: me@dreamyouxi.com

]]


local t = { }

local log = require("log"):new("login")
local remote = require("xygame.base.remote");

global_players_ctx = { };


function t.login(ctx, msg, cb)
    local kv = json.decode(msg);

    -- 新连接请求
    remote.request_client(ctx, "Room", "SelfEnterRoom", "no:" .. kv["name"] .. ",", function(msg)


        if msg == "timeout" then return end;
        --- 响应失败 忽略

        print("Enself room " .. kv["name"] .. "  recv respone: " .. msg);

        -- 响应成功后 添加到table里面
        table.insert(global_players_ctx, ctx);

    end );
    cb("ret:ok,");
end



return t;