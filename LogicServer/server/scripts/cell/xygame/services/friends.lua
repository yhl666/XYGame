
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

-- 添加好友，根据用户no
function t.add_by_no(msg, cb)
    local kv = json.decode(msg);
    local no = kv["my_no"];
    local no_target = kv["no_target"];
    msg = "no:" .. no_target .. ",";

    remote.request_local("services.user", "query_user", msg, function (msg)
        local kv = json.decode(msg);
        if "ok" == kv["ret"] then
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

            end);
            
        else
        
            cb("ret:error,msg:该用户不存在,");
        end
    end);
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
            cb(""); return;
        end
        remote.request_local("services.user", "query_by_list", msg, function(msg)

            cb(msg);
        end );

    end );

end



function t.delete_by_no(msg, cb)

    local kv = json.decode(msg);
    local no = kv["no"];
    local who = kv["who"];

    redis.get(redis_key.get_friends(no), function(msg)
        if msg == "" then
            -- 没有好友
            cb("ret:error,msg:你没有好友,"); return;
        end
        if string.find(msg, "no:" .. who .. ",") == nil then

            -- 没有好友
            cb("ret:error,msg:你们不是好友"); return;
        end
        print("reg11 " .. msg);
        msg =(string.gsub(msg, "{no:" .. who .. ",}", ""));
        print("reg " .. msg);

        if msg == "" then
            msg = "\"\""; --没有好友了

        end
        redis.set(redis_key.get_friends(no), msg .. "", function(res)
            if res == "" then

                cb("ret:error,msg:删除出错,");
            else
                cb("ret:ok,msg:删除成功,");
            end

        end );


    end );
end







return t;