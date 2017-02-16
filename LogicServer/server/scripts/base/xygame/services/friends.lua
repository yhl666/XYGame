
--[[

* Author: caoshanshan
* Email: me@dreamyouxi.com

]]


local t = { }

local log = require("log"):new("friends")
local remote = require("base.remote");

-- 删除好友
function t.remove(ctx, msg, cb)
    -- 发起cell服务器的远程调用，请求 login
    remote.request("services.friends", "remove", msg, function(msg)
        if msg == "timeout" then
            cb("ret:error,msg:timeout,");
            return;
        end
        cb(msg);
    end );
end

-- 添加好友
function t.add(ctx, msg, cb)

    -- 发起cell服务器的远程调用，请求 register
    remote.request("services.friends", "add", msg, function(msg)
        if msg == "timeout" then
            cb("ret:error,msg:timeout,");
            return;
        end
        cb(msg);
    end );



   local ret=   get_friends  ();
   if ret == "" then

   ebd


end



-- 发消息
function t.send(ctx, msg, cb)
    local kv = json.decode(msg);
    local no = kv["no"];
    local who = kv["who"];
    local content = kv["msg"];

    -- 先查询是否有该好友，
    remote.request_local("services.friends", "query", "no:" .. no .. ",who:" .. who .. ",", function(res)

        if res == "" then
            cb("ret:error,msg:" .. " 你们还不是好友关系,");
        else
            local hero = global_hero_list:get_hero_by_no(no);

            if hero == nil then
                cb("ret:error,msg:" .. " 你的好友不在线,");
            else
                cb("ret:ok,");
                -- remote.request_client ();



            end
        end

    end );
end






return t;