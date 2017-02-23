
--[[

* Author:  sutao
* Email:   sutao@ztgame.com


--{no:1,no_target:2,}no我的id no_target 我要战的id
--request_pvp_v1(ctx, msg, cb)



--{hp:50,no:1,}{hp:0,no:0,} -- 第一个默认为1号玩家
--request_verify()
end
]]


local t = { }

local log = require("log"):new("login")
local remote = require("xygame.base.remote");
local hero = require("model.base_hero")
local queue = require("base.queue");
local user = require("model.user")


t.enable_hotfix = false;  -- 禁止hot fix

global_ramdon_queue_v1 = queue.new();-- 全局队列
global_ramdon_queue_v2 = queue.new();-- 全局队列

-- [Common]
-- {no:1,no_target:2,}
function t.request_pvp_v1(ctx, msg, cb)

    remote.request("services.battle_pvp", "request_pvp_v1", msg, function(msg1)
        if msg == "" then
            cb("ret:error,msg:timeout,");
            return;
        end
        cb(msg1);
    end );
end

function t.request_pvp_v2(ctx, msg, cb)
    local kv = json.decode(msg);
    local no = kv["no"];
    local no_target = kv["no_target"];

    local msg1 = "no:" .. no .. ",";
    local msg2 = "no:" .. no_target .. ",";

    local online = false;
    local ctx_2;

    global_hero_list:foreach( function(k, v)

        if v.user.no == no_target then
            -- p2玩家在线
            ctx_2 = v.ctx;
            online = true;
        end
    end );

    if online == false then
        cb("ret:error,msg:该玩家不在线,"); return;
    end

    remote.request("services.user", "query_user", msg2, function(msg5)
        if json.decode(msg5)["ret"] == "ok" then

            local user2 = string.gsub(msg5, "ret:ok,", "");

            remote.request("services.user", "query_user", msg1, function(msg5)

                if json.decode(msg5)["ret"] == "ok" then
                    local user1 = string.gsub(msg5, "ret:ok,", "");

                    remote.request("services.battle_pvp", "request_pvp_v2", msg, function(msg5)
                        if msg == "tiemout" then
                            cb("ret:error,msg:timeout,");
                            return;
                        end
                        remote.request_client(ctx_2, "Friends", "RecvPVP", user1 .. msg5, function(msg)

                            local kv = json.decode(msg);
                            if kv["ret"] == "ok" then
                                cb(user2 .. msg5);
                            else
                                cb("ret:error,msg:玩家拒绝,");
                            end
                        end );
                    end );
                else

                    cb("ret:error,msg:该玩家不存在,");
                end
            end );
        else

            cb("ret:error,msg:该玩家不存在,");
        end
    end );
end

function t.request_verify(ctx, msg, cb)

    local tbl = json.decode(msg);
    local pvproom_id = tbl["pvproom_id"];

    local p1 = tbl["p1"];
    local p2 = tbl["p2"];
    local msg1 = "pvproom_id:" .. pvproom_id .. ",";

    -- 参数校验
    local ctx_other;
    global_hero_list:foreach( function(k, v)

        if v.user.no == p1 and ctx:get_rpc_clt_id() ~= v.ctx:get_rpc_clt_id() then
            ctx_other = v.ctx;

        end

        if v.user.no == p2 and ctx:get_rpc_clt_id() ~= v.ctx:get_rpc_clt_id() then
            ctx_other = v.ctx;

        end
    end );

    remote.request("services.battle_pvp", "request_verify_read", msg1, function(msg2)

        msg2 = string.gsub(msg2, "ret:ok,", "");
        msg2 = string.gsub(msg2, "max_no:2,", "");
        -- ret:ok是读取成功之后引入的
        local tbl_battle_in_redis = json.decode(msg2);

        if tbl_battle_in_redis["ret"] == "error" then
            cb("ret:error,msg:房间数据错误,"); return;
        else
            if tbl["pvproom_id"] ~= tbl_battle_in_redis["pvproom_id"] or tbl["p1"] ~= tbl_battle_in_redis["p1"] or
                tbl["p2"] ~= tbl_battle_in_redis["p2"] then
                -- todo
                -- 如果房间信息有误则通知两个玩家游戏结果校验失败
                remote.request_client(ctx, "Friends", "BattlePVP", "ret:ok,msg:verifyError,", function(msg3)

                end );

                remote.request_client(ctx_other, "Friends", "BattlePVP", "ret:ok,msg:verifyError,", function(msg3)

                end );

            else
                if string.find(msg2, "h1") == nil or string.find(msg2, "h2") == nil then
                    -- 暂时做一个简单的判断处理没有血量的情况
                    -- 如果是一号玩家
                    local msg_to_write = msg2 .. "h1:" .. tbl["h1"] .. "," .. "h2:" .. tbl["h2"] .. ",";
                    remote.request("services.battle_pvp", "request_verify_write", msg_to_write, function(msg4)
                        -- 讲血量写入数据库

                        local kvv = json.decode(msg4);
                        if kvv["ret"] == "ok" then
                            -- 已经写入redis
                            cb("ret:ok,msg:写入成功,");
                        else
                            cb("ret:ok,msg:写入失败,");
                            -- 写入redis失败
                        end

                    end )

                else

                    -- 如果是二号玩家
                    cb("ret:ok,");
                    if msg == msg2 then
                        -- 二号发过来的结果数据和数据库中取得一致则

                        -- 发给客户服务器发送房间信息
                        local room_info = "pvproom_id:" .. pvproom_id .. "," .. "p1:" .. p1 .. "," .. "p2:" .. p2 .. ",";

                        remote.request_client_server(room_info, function(msgg)

                            local kvv = json.decode(msgg);

                            if tbl_battle_in_redis["h1"] == kvv["h1"] and tbl_battle_in_redis["h2"] == kvv["h2"] then

                                remote.request_client(ctx, "BattlePVP", "PushResult", "ret:ok,msg:verifyOk,", function(msg6)
                                    -- Friend?
                                end );

                                remote.request_client(ctx_other, "BattlePVP", "PushResult", "ret:ok,msg:verifyOk,", function(msg6)
                                    -- Friend?
                                end );
                            else
                                remote.request_client(ctx, "BattlePVP", "PushResult", "ret:error,msg:verifyError,", function(msg6)
                                    -- Friend?
                                end );

                                remote.request_client(ctx_other, "BattlePVP", "PushResult", "ret:error,msg:verifyError,", function(msg6)
                                    -- Friend?
                                end );
                            end
                        end )

                    else

                        remote.request_client(ctx, "BattlePVP", "PushResult", "ret:error,msg:verifyError,", function(msg6)
                            -- Friend?
                        end );

                        remote.request_client(ctx_other, "BattlePVP", "PushResult", "ret:error,msg:verifyError,", function(msg6)
                            -- Friend?
                        end );

                    end

                end


            end

        end
    end );
end

-- 进入 1v1的随机匹配
-- msg = no;1,
function t.request_pvp_ramdon_enter_queue_v1(ctx, msg, cb)
    
    local kv = json.decode(msg);
    local no = kv["no"];
    local msg1 = "no:" .. no .. ",";

    local queue_size = global_ramdon_queue_v1:size();
    --local hero_frist;
    --匹配的第一个玩家
    if queue_size < 1 then
        
        global_hero_list:foreach(function (k, v)
            
            if v.user.no == no then
                global_ramdon_queue_v1:push(v);
                cb("ret:ok,msg:你已经进入游戏队列");
            else
                cb("ret:ok,msg:你的玩家信息有误,");return;
            end

        end)
    else
        --匹配的第二个玩家
        local u;
        global_hero_list:foreach(function (k, v)
            if v.user.no == no then
                u = v;
            else
                cb("ret:ok,msg:你的信息有误,"); --传入的玩家二的no信息有误
            end

        end)
        

        local v = global_ramdon_queue_v1:front();
        local msg_to_cell = "p1:" .. v.no .. "," .. "p2:" .. no .. ",";
        remote.request("service.battle_pvp", "request_pvp_ramdon_enter_queue_v1", msg_to_cell, function (msg_from_cell)

            local kvv = json.decode(msg_from_cell);            
            local msg2 = "pvproom_id:" .. kvv["pvproom_id"].. "," .. "max_no:2" .. "," .. "p1:" .. kvv["p1"] .. "," .. "p2:" .. kvv["p2"] .. ",";
            global_ramdon_queue_v1:pop();


            remote.request_client(v.ctx, "TownBattlePvP", "PushResult", msg2 .. v.user.tostring(), function (msg3)           --todo
                
                local kvvv = json.decode(msg3);
                if kvvv["ret"] == "ok" then
                    cb(msg2 .. u.user.tostring());
                else
                    cb("ret:error,msg:匹配失败,");
                    global_hero_list:foreach(function (k, v)
            
                        if v.user.no == no then
                            global_ramdon_queue_v1:push(v);
                            cb("ret:ok,msg:你已经进入游戏队列");
                        else
                            cb("ret:ok,msg:你的玩家信息有误,");return;
                        end

                    end)
                end
            end );    

        end)
     
    end
end


local function get_hero_from_global_list(list, no, hero)
    
    list:foreach(function (k, v)
        if v.user.no == no then 
           hero = v;
        else
            hero = nil;
        end 
    end)

    return hero;
end

-- 离开 1v1的随机匹配
-- msg = no;1,
function t.request_pvp_ramdon_leave_queue_v1(ctx, msg, cb)

    local kv = json.decode(msg);
    local no = kv["no"];
    
    
    global_ramdon_queue_v1:foreach(function(k, v) 
        
        if k.user.no == no then
            global_ramdon_queue_v1:front();
            cb("ret:ok,msg:删除成功,");
        else
            cb("ret:error,msg:退出房间有误,");
        end
    end)
end




-- 进入 1v1的随机匹配
-- msg = no;1,
function t.request_pvp_ramdon_enter_queue_v2(ctx, msg, cb)

    cb("ret:ok,");

end
-- 离开 1v1的随机匹配
-- msg = no;1,
function t.request_pvp_ramdon_leave_queue_v2(ctx, msg, cb)

    cb("ret:ok,");


end

return t;