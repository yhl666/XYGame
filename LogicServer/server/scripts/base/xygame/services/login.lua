
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
        print("Enself room " .. kv["name"] ..  "  recv respone: " .. msg);


        if msg == "" then return end;
        --- ��Ӧʧ�� ����
        print("Enself room " .. kv["name"] .. "  ok");


        -- �ɹ��� ֪ͨ�����������
        for k, v in pairs(global_players_ctx) do

            remote.request_client(v, "Room", "EnterRoom", "no:" .. kv["name"] .. ",", function(msg)
                if msg == "" then
                    -- ��������뿪���� �Ƴ��б�
                    print("֪ͨ�������ʧ��");
                    return;
                end
            end )

        end

        -- ��Ӧ�ɹ��� ��ӵ�table����
        table.insert(global_players_ctx, ctx);

    end );
    cb("ret:ok");
end



return t;