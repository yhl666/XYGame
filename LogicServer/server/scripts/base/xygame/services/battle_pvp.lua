
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
        local kv = json.decode(msg);
        room_leave_room_by_no(kv["no"]);
        room_leave_room_by_no(kv["no_target"]);


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

                                room_leave_room_by_no(no);
                                room_leave_room_by_no(no_target);

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
    global_online_hero_list:foreach( function(k, v)

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
                cb("ret:ok,");
                remote.request_client(ctx, "BattlePVP", "PushResult", "ret:error,msg:verifyError,", function(msg3)

                end );

                remote.request_client(ctx_other, "BattlePVP", "PushResult", "ret:error,msg:verifyError,", function(msg3)

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


local function notify_client(user, msg, no)

    remote.request_client(user.ctx, "TownBattlePVP", "PushResult", msg, function(msg3)
        if msg3 == "timeout" then

            if user.user.is_dirty == true then
            else
                global_ramdon_queue_v1:push(no);
            end
        end

        local kv = json.decode(msg3);
        if kv["ret"] == "ok" then
          room_leave_room_by_no(no);
     
        else
            global_ramdon_queue_v1:push(no);
        end

    end );

end

-- 进入 1v1的随机匹配
-- msg = no;1,
function t.request_pvp_ramdon_enter_queue_v2(ctx, msg, cb)

    local kv = json.decode(msg);
    local no = kv["no"];

    if global_ramdon_queue_v1:size() >= 1 then
        -- 第二个玩家
        local no1 = global_ramdon_queue_v1:front();
        local no2 = no;

        local u1 = global_hero_list:get_hero_by_no(no1);
        if u1 == nil or no1 == no2 then
            global_ramdon_queue_v1:push(no2);
            cb("ret:ok,");
            return;
        end
        local user1 = nil;
        local user2 = nil;

        global_hero_list:foreach( function(k, v)
            if no1 == v.user.no then
                user1 = v;
            end
            if no2 == v.user.no then
                user2 = v;
            end

        end );


        remote.request("services.battle_pvp", "request_pvp_v2", "no:" .. no1 .. ",no_target:" .. no2 .. ",", function(msg5)

            if msg == "tiemout" then
                cb("ret:error,msg:timeout,");
                return;
            end


            msg5 = string.gsub(msg5, "ret:ok,", "");


            local u1 = global_hero_list:get_hero_by_no(no1);
            local u2 = global_hero_list:get_hero_by_no(no2)


            if u1 == nil and u2 == nil then
                return;
            end

            if u2 == nil then
                global_ramdon_queue_v1:push(no1); return;
            end
            cb("ret:ok,");
            if u1 == nil then
                global_ramdon_queue_v1:push(no2); return;
            end

            notify_client(user1, msg5 .. user2.user:to_json(), no1);
            notify_client(user2, msg5 .. user1.user:to_json(), no2);


        end );

    else
        -- 第一个玩家
        global_ramdon_queue_v1:push(no);
        cb("ret:ok,msg:进入队列成功,");
    end




end


local function get_hero_from_global_list(list, no, hero)

    list:foreach( function(k, v)
        if v.user.no == no then
            hero = v;
        else
            hero = nil;
        end
    end )

    return hero;
end

-- 离开 1v1的随机匹配
-- msg = no;1,
function t.request_pvp_ramdon_leave_queue_v2(ctx, msg, cb)

    local kv = json.decode(msg);
    local no = kv["no"];
    local size = global_ramdon_queue_v1:size();

    print("current size = " .. size);
    for i = 0, size do

        local noo = global_ramdon_queue_v1:front();
        if noo == no then

            cb("ret:ok,"); return;
        else
            global_ramdon_queue_v1:push(noo);
        end

    end

    cb("ret:error,");
end




-- 进入 1v1的随机匹配
-- msg = no;1,
function t.request_pvp_ramdon_enter_queue_v3(ctx, msg, cb)

    cb("ret:ok,");

end
-- 离开 1v1的随机匹配
-- msg = no;1,
function t.request_pvp_ramdon_leave_queue_v3(ctx, msg, cb)

    cb("ret:ok,");


end


 


return t;