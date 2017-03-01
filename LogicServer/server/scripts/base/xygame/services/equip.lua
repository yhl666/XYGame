
--[[

* Author:  sutao
* Email:   sutao@ztgame.com

]]


local t = { }

local log = require("log"):new("equip")
local remote = require("xygame.base.remote");

--给装备增加经验
function t.add_exp(ctx, msg, cb)

    remote.request("services.equip", "add_exp", msg, function(msg)
        if msg == "timeout" then
            cb("ret:error,msg:timeout,");
            return;
        end
        cb(msg);
    end );
end

-- 产生一件装备
-- ownner 拥有者id，id 装备id
function t.gen_a_random(ctx, msg, cb)

    remote.request("services.equip", "gen_a_random", msg, function(msg)
        if msg == "timeout" then
            cb("ret:error,msg:timeout,");
            return;
        end
        cb(msg);
    end );
end


function t.query_all(ctx, msg, cb)

    remote.request("services.equip", "query_all", msg, function(msg)
        if msg == "timeout" then
            cb("ret:error,msg:timeout,");
            return;
        end
        cb(msg);
    end );
end

-- 查询一条装备信息
-- no
-- ownner
function t.query(ctx, msg, cb)

    remote.request("services.equip", "query", msg, function(msg)
        if msg == "timeout" then
            cb("ret:error,msg:timeout,");
            return;
        end
        cb(msg);
    end );
end

return t;