
--[[
登录服务
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

    -- 新连接请求
    remote.request_client(ctx, "Room", "SelfEnterRoom", "no:" .. kv["name"] .. ",", function(msg)

        if msg == "timeout" then return end;
        --- 响应失败 忽略

        print("hero join game no=" .. kv["name"]);

        -- 响应成功后 添加到table里面
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