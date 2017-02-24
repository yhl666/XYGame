
--[[
登录服务
* Author: caoshanshan
* Email: me@dreamyouxi.com

]]


local t = { }

local log = require("log"):new("login")
local remote = require("base.remote");


local hero = require("model.base_hero")
local hero_list = require("model.hero_list")
local user = require("model.user")
local t = { }

t.enable_hotfix = false;  -- 禁止hot fix

global_online_hero_list = hero_list.new();




function t.login(ctx, msg, cb)
    -- 发起cell服务器的远程调用，请求 login
    remote.request("services.login", "login", msg, function(msg)
        if msg == "timeout" then
            cb("ret:error,msg:timeout,");
            return;
        end
        cb(msg);
        local user1 = user.create();
        msg = string.gsub(msg, "ret:ok,", "");
        user1:set_json(msg);
        global_online_hero_list:add(hero.create(ctx, user1));
    end );
end


 

function t.register(ctx, msg, cb)

    -- 发起cell服务器的远程调用，请求 register
    remote.request("services.login", "register", msg, function(msg)
        if msg == "timeout" then
            cb("ret:error,msg:timeout,");
            return;
        end
        cb(msg);
    end );
end



 


return t;