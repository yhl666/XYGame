
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
function t.add( msg, cb)

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




return t;