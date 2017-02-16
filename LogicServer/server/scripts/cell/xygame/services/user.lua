
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


return t;