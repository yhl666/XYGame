
--[[
redis helper modele
* Author: caoshanshan
* Email: me@dreamyouxi.com

]]


local t = { }



-- [Comment]
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



-- [Comment]
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



-- [Comment]
-- 执行redis命令
--  回调的参数为简单json
-- ret:ok,msg:XXX,  表示成功 ，。msg为redis返回的信息
-- ret:error,msg:XXX,  表示失败，。msg为redis返回的信息
function t.exec(cmd, cb)
    c_redis.exec(cmd, function(msg)
        cb(msg);

    end )
end




return t;