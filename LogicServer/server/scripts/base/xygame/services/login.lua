
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

    local account = kv["account"];
    local pwd = kv["pwd"];

    redis.get("login-" .. account, function(msg)

        if msg == "" then
            cb("ret:账户不存在,"); return;
        end
        local kvv = json.decode(msg);

        if pwd == kvv["pwd"] then
            -- 登陆成功
            cb("ret:ok,name:" .. kvv["name"] .. ",");
        else
            -- 账户密码错误
            cb("ret:密码或账户错误,");
        end




    end );

end





function t.register(ctx, msg, cb)
    local kv = json.decode(msg);
    local name = kv["name"];
    local account = kv["account"];
    local pwd = kv["pwd"];


    -- 先查询账户存在否

    redis.get("login-" .. account, function(msg)

        if msg == "" then
            --- "账户不存在
            redis.set("login-" .. account, "pwd:" .. pwd .. "," .. "name:" .. name .. ",", function(msg)

                if msg == "ok" then
                    cb("ret:ok"); return;
                end
                cb("ret:服务器内部错误，注册失败,");
            end );
        else

            cb("ret:账户已存在,");

        end


    end );

end




return t;