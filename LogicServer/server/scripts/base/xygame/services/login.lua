
--[[
��¼����
* Author: caoshanshan
* Email: me@dreamyouxi.com

]]


local t = { }

local log = require("log"):new("login")
local remote = require("xygame.base.remote");

global_players_ctx = { };


function t.login(ctx, msg, cb)
    local kv = json.decode(msg);

    -- ����������
    remote.request_client(ctx, "Room", "SelfEnterRoom", "no:" .. kv["name"] .. ",", function(msg)


        if msg == "timeout" then return end;
        --- ��Ӧʧ�� ����

        print("Enself room " .. kv["name"] .. "  recv respone: " .. msg);

        -- ��Ӧ�ɹ��� ��ӵ�table����
        table.insert(global_players_ctx, ctx);

    end );
    cb("ret:ok,");
end



return t;