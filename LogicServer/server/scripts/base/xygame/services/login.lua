
--[[
登录服务
* Author: caoshanshan
* Email: me@dreamyouxi.com

]]


local t = { }

local log = require("log"):new("login")
local remote = require("base.remote");

function t.login(ctx, msg, cb)
    -- 发起cell服务器的远程调用，请求 login
    remote.request("services.login", "login", msg, function(msg)
        if msg == "timeout" then
            cb("ret:error,msg:timeout,");
            return;
        end
        cb(msg);
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