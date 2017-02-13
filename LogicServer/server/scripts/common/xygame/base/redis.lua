
--[[
redis helper modele
* Author: caoshanshan
* Email: me@dreamyouxi.com

]]


local t = { }



--[Comment]
-- 参数为""表示失败
-- "ok" 表示成功
function t.set(key, value, cb)

    c_redis.set(key, value, function(msg)
        if msg == "OK" then
            cb("ok");
        else
            cb("");
        end

    end )
end



--[Comment]
-- 参数为""表示失败
-- 其他表示 获取的值 表示成功
function t.get(key, cb)

    c_redis.get(key, function(msg)
        if msg == "" then
            cb("");
        else
            cb(msg);
        end

    end )
end





return t;