
--[[
��¼����
* Author: caoshanshan
* Email: me@dreamyouxi.com

]]


local t = { }

local log = require("log"):new("login")
local remote = require("base.remote");
local hero = require("model.base_hero")
local redis = require("base.redis");

global_base_heros = { };


function t.login(ctx, msg, cb)
    local kv = json.decode(msg);

    -- ����������
    remote.request_client(ctx, "Room", "SelfEnterRoom", "no:" .. kv["name"] .. ",", function(msg)

        if msg == "timeout" then return end;
        --- ��Ӧʧ�� ����

        print("hero join game no=" .. kv["name"]);

        -- ��Ӧ�ɹ��� ��ӵ�table����
        table.insert(global_base_heros, hero.create(ctx, kv["name"]));

    end );


    
   redis.get("ssqgqwegss"  ,function (msg)
       print(msg);
    end);

       
   redis.set("ssss"  ,"ewgewewghewewhew",function (msg)
       print(msg);
    end);


    cb("ret:ok,");
end


return t;