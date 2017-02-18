
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
    local no = kv["no"];
    local who = kv["who"];
    msg = "no:" .. who .. ",";

    remote.request_local("services.user", "query_user", msg, function (msg)
        local kv = json.decode(msg);
        if "ok" == kv["ret"] then
            local user_msg = msg;

            redis.get(redis_key.get_friends(no), function(msg)

                if msg == "" then
                    
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



--删除好友
function t.delete_by_no(msg, cb)
    
    local kv = json.decode(msg);
    local no = kv["no"];
    local who = kv["who"];  --好友的no

    redis.get(redis_key.get_friends(no), function(msg2)
        if "" == msg2 then
            cb("ret:error, msg:你的好友列表为空,");return;

        else
            if string.find(msg2, "no:" .. who .. ",") == nil then 
                cb("ret:error,msg:该好友不存在,");return;

            else
                msg2 = string.gsub(msg2, "{no:" .. who .. ",}", ""); --将目标的信息设置为空

                if msg2 == "" then
                    redis.exec("del " .. redis_key.get_friends(no), function(msgg)

                        local kvv = json.decode(msgg);
                        if (kvv["ret"] ~= "ok") then
                            cb("ret:error, msg:delete failed,"); return;     -- 删除失败 或者 无该key
                        else
                            cb("ret:ok, msg:delete success,"); return;
                            --   删除成功
                        end
                    
                    end);
                end
                
                redis.set(redis_key.get_friends(no), msg2, function(msg3)
                    
                    if msg3 == "ok" then       
                        cb("ret:ok, msg:delete success,");return;
                    else
                        cb("ret:error, msg:delete failed,");
                    end
                        
                end);               
            end    
        end
    
    end);

end








return t;