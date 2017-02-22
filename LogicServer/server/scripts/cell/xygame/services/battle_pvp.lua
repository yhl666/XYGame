--[[

* Author:  sutao
* Email:   sutao@ztgame.com


--{no:1,no_target:2,}no我的id no_target 我要战的id
--request_pvp_v1(ctx, msg, cb)

end
]]

local t = { }

local log = require("log"):new("login")
local remote = require("base.remote");
local redis = require("base.redis");
local redis_key = require("utils.redis_key")
local user = require("model.user");
local hero = require("model.base_hero");

function t.request_pvp_v1(msg, cb)
    
    local kv = json.decode(msg);
    local no = kv["no"];
    local no_target = kv["no_target"];

    msg1 = "no:" .. no_target .. ",";
    remote.request_local("services.user", "query_user", msg1, function(msg2)

        local kv = json.decode(msg2);
        if kv["ret"] == "error" then 
            cb("ret:error,msg:没有该玩家的用户信息,"); return; --没有对手的用户信息
        else 
            redis.exec("incr GLOBAL_MAX_BATTLE_ROOM_NO", function(msg3)

                print("exec redis cmd: " .. msg3);
                local kvv = json.decode(msg3);
                local id = kvv["msg"];

                local msg = "pvproom_id:" .. id.. "," .. "max_no:1" .. "," .. "p1:" .. no .. "," .. "p2:" .. no_target .. ",";
                msg2 =msg2 .. "pvproom_id:" .. id .. "," .. "max_no:1,";

                if (kvv["ret"] == "ok") then
                -- 自动增长ID成功
                    redis.set(redis_key.get_pvproom(id), msg, function(msg1)
        
                        if msg1 == "ok" then 
                            cb(msg2);
                        else
                            cb("ret:error,msg:服务器内部错误，申请战斗失败,");
                        end
                    end);
                end

            end);         
        end
    end);
end


function t.request_pvp_v2(msg, cb)

    local kv = json.decode(msg);
    local no = kv["no"];
    local no_target = kv["no_target"];

    msg1 = "no:" .. no_target .. ","
            
    redis.exec("incr GLOBAL_MAX_BATTLE_ROOM_NO", function(msg3)

        print("exec redis cmd: " .. msg3);
        local kvv = json.decode(msg3);
        local id = kvv["msg"];

        local msg = "pvproom_id:" .. id.. "," .. "max_no:2" .. "," .. "p1:" .. no .. "," .. "p2:" .. no_target .. ",";

        if (kvv["ret"] == "ok") then
            -- 自动增长ID成功
            redis.set(redis_key.get_pvproom(id), msg, function(msg1)
        
                if msg1 == "ok" then 
                    cb("ret:ok,"..msg);
                else
                    cb("ret:error,msg:服务器内部错误，申请战斗失败,");
                end
            end);
        end

    end);  
end


 --通知检验比赛结果
 function t.request_verify_read(msg, cb)

    local kv =json.decode(msg);
    local pvproom_id = kv["pvproom_id"];

    redis.get(redis_key.get_pvproom(pvproom_id), function (msg)
        if msg == "" then
            cb("ret:error,msg:战斗房间为空,");
            return;
        else
            cb("ret:ok,"  .. msg);
        end

    end);
end


function t.request_verify_write(msg, cb)

    local tbl = json.multi_decode(msg);
    local pvproom_id = tbl[1]["pvproom_id"];

    redis.set(redis_key.get_pvproom(pvproom_id), msg, function(msg1)
        if msg1 == "ok" then 
            cb("ret:ok,"); 
        else
            cb("ret:error,msg:数据写入失败,");return;  
        end
    end)

end

return t;
