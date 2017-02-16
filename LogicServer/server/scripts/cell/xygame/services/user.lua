
--[[

* Author: caoshanshan
* Email: me@dreamyouxi.com

]]


local t = { }

local log = require("log"):new("user")
local remote = require("base.remote");

local redis = require("base.redis");
local redis_key = require("utils.redis_key")
local user = require("model.user");



function t.add_user(msg, cb)

    local kv = json.decode(msg);
    local no = kv["no"];
    local name = kv["name"];


    local user1 = user.create();
    user1.no = no;
    user1.time = os.time();
    user1.name = name;

    redis.set(redis_key.get_user(no), user1:to_json(), function(msg)

        if msg == "ok" then
            -- 新建user 成功
            cb("ret:ok,"); return;
        end

        -- 新建user失败

        cb("ret:error,");


    end );
end



function t.query_user(msg, cb)
    local kv = json.decode(msg);
    local no = kv["no"];

    redis.get(redis_key.get_user(no), function(msg)

        if msg == "" then
            cb("ret:error,");
        else
            cb("ret:ok," .. msg);
        end
    end );

end
 
function t.query_by_name(msg, cb, no, limit)

    if no == nil then
        no = 0;
        no = no + 1;
        redis.get("GLOBAL_MAX_NO", function(msg1)

            t.query_by_name(msg, cb, no, tonumber(msg1))
        end );

        return;
    end


    if limit == no then

        cb("ret:error,msg:" .. "用户不存在,"); return;
    end


    print(" query " .. no .. "   " .. limit)

    local kv = json.decode(msg);
    local name = kv["name"];

    redis.get(redis_key.get_user(no), function(msg1)

        if msg1 == "" then

            t.query_by_name(msg, cb, no + 1, limit)
        else
            local user1 = user:create();
            user1:set_json(msg1);
            if user1.name == json.decode(msg).name then
                cb("ret:ok," .. msg1);
            else
                t.query_by_name(msg, cb, no + 1, limit)
            end
        end
    end );




end



return t;