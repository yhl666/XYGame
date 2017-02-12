
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
        print("Enself room " .. kv["name"] ..  "  recv respone: " .. msg);


        if msg == "" then return end;
        --- 响应失败 忽略
        print("Enself room " .. kv["name"] .. "  ok");


        -- 成功后 通知其他在线玩家
        for k, v in pairs(global_players_ctx) do

            remote.request_client(v, "Room", "EnterRoom", "no:" .. kv["name"] .. ",", function(msg)
                if msg == "" then
                    -- 该玩家已离开房间 移除列表
                    print("通知其他玩家失败");
                    return;
                end
            end )

        end

        -- 响应成功后 添加到table里面
        table.insert(global_players_ctx, ctx);

    end );
    cb("ret:ok");
end



return t;