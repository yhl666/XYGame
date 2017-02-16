
--[[
登录服务
* Author: caoshanshan
* Email: me@dreamyouxi.com

]]


local t = { }

local log = require("log"):new("login")
local remote = require("base.remote");
local hero = require("model.base_hero")
local redis = require("base.redis");
local redis_key = require("utils.redis_key")
local user = require("model.user");


function t.login(msg, cb)
    local kv = json.decode(msg);

    local account = kv["account"];
    local pwd = kv["pwd"];

    redis.get(redis_key.get_login(account), function(msg)

        if msg == "" then
            cb("ret:账户不存在,"); return;
        end
        local kvv = json.decode(msg);
        local no = kvv["no"]
        if pwd == kvv["pwd"] then
            -- 账户密码验证正确

            -- 查询user
            redis.get(redis_key.get_user(no), function(msg)

                if msg == "" then
                    cb("ret:内部错误登陆失败,"); return;
                    -- 查询user失败
                end

                local user1 = user:new();

                user1:set_json(msg);
                user1.time = os.time();
                -- 更新登陆时间

                local str = user1:to_json();

                cb("ret:ok,msg:" .. str .. ",");
                -- 查询成功返回user信息

                redis.set(redis_key.get_user(no), str, function(msg)
                    if msg == "" then
                        -- 更新登陆时间失败
                    end

                end );


            end );

        else
            -- 账户密码错误
            cb("ret:密码或账户错误,");
        end

    end );

end


local function do_register(id, kv, cb)

    local name = kv["name"];
    local account = kv["account"];
    local pwd = kv["pwd"];

    local param = "account:" .. account .. ",no:" .. id .. ",pwd:" .. pwd .. ",name:" .. name .. ",";
    redis.set(redis_key.get_login(account), param, function(msg)

        if msg == "ok" then
            -- 插入login 信息成功
            --- 插入user 表
            local user1 = user:new();
            user1.time = os.time();
            user1.no = id;
            redis.set(redis_key.get_user(id), user1:to_json(), function(msg)

                if msg == "ok" then
                    -- 插入user 成功
                    cb("ret:ok,msg:注册成功,"); return;
                end

                -- 插入user失败  回滚插入 login 操作

                cb("ret:error,msg:服务器内部错误 插入user失败，注册失败,");
                redis.exec("del " .. redis_key.get_login(account), function(msggg)

                    local kvv = json.decode(msggg);
                    if (kvv["ret"] == "ok") then
                        --   删除成功
                    else
                        -- 删除失败 或者 无该key
                    end

                end )

            end );



        else
            -- 插入login 失败
            cb("ret:error,msg:服务器内部错误，插入login失败注册失败,");
        end
    end );
end


function t.register(msg, cb)
    local kv = json.decode(msg);
    local account = kv["account"];

    -- 先查询账户存在否

    redis.get(redis_key.get_login(account), function(msg)

        if msg == "" then
            --- "账户不存在
            redis.exec("incr GLOBAL_MAX_NO", function(msggg)
                print("exec redis cmd: " .. msggg);
                local kvv = json.decode(msggg);
                if (kvv["ret"] == "ok") then
                    -- 自动增长ID成功
                    do_register(kvv["msg"], kv, cb);

                end

            end )
        else

            cb("ret:error,msg:账户已存在,");

        end

    end );

end



 


return t;