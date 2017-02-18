
--[[

* Author: caoshanshan
* Email: me@dreamyouxi.com

]]


local t = { }

local log = require("log"):new("friends")
local remote = require("base.remote");

local redis = require("base.redis");
local redis_key = require("utils.redis_key")

-- 删除好友
function t.remove(msg, cb)

end

-- 添加好友
function t.add(msg, cb)

    local kv = json.decode(msg);
    local no = kv["no"];
    local who_no = kv["who"];



    redis.get(redis_key.get_friends(no), function(res)
        if res == "" then

            cb("");
        else
            cb(res);
        end

    end );



end





-- 添加好友
function t.add_by_name(msg, cb)

    local kv = json.decode(msg);
    local no = kv["no"];
    local name = kv["name"];

    remote.request_local("services.user", "query_by_name", msg, function(msg)

        local kv = json.decode(msg);
        if kv["ret"] == "ok" then
            local user_msg = msg;

            redis.get(redis_key.get_friends(no), function(msg)
                if msg == "" then
                    -- 没有好友

                else
                    -- 有好友
                    if string.find(msg, kv["no"]) ~= nil then

                        cb("ret:error,msg:已经是你的好友,"); return;
                    end

                end

                msg = msg .. "{no:" .. kv["no"] .. ",}";

                redis.set(redis_key.get_friends(no), msg, function(msg1)
                    if msg1 == "ok" then
                        cb(user_msg)
                    else
                        cb("ret:error,msg:服务器内部错误");
                    end

                end )

            end );

        else

            cb("ret:error,msg:该用户不存在,");
        end

    end )

end

-- 查询
function t.query(msg, cb)
    local kv = json.decode(msg);
    local no = kv["no"];
    local who_no = kv["who"];
    redis.get(redis_key.get_friends(no), function(res)
        if res == "" then

            cb("");
        else
            cb(res);
        end

    end );

end











-- 添加好友
function t.query_all(msg, cb)

    local kv = json.decode(msg);
    local no = kv["no"];


    redis.get(redis_key.get_friends(no), function(msg)
        if msg == "" then
            -- 没有好友
            cb("ret:error,"); return;
        end

        remote.request_local("services.user", "query_by_list", msg, function(msg)

        print("cell ret " .. msg)
            cb(msg);
        end );

    end );


end











return t;