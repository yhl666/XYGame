
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
local remote = require("base.remote");
local hero = require("model.base_hero")

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
    local msg1 = "pvproom_id:" .. pvproom_id .. ",";
    local p1 = tbl["p1"];
    local p2 = tbl["p2"];

    -- 参数校验

    -- 约定{pvproomid:1,max_no:2,p1:1,p2:2}{hp:100,p1:1}{hp:100,p2:2},消息发送者为p1,对手为p2
    local ctx_other;

    remote.request("services.battle_pvp", "request_verify_read", msg1, function(msg2)


    end );
end


function t.request_verify1111111111111111(ctx, msg, cb)

    local tbl = json.decode(msg);
    local pvproom_id = tbl["pvproom_id"];
    local msg1 = "pvproom_id:" .. pvproom_id .. ",";
    local p1 = tbl["p1"];
    local p2 = tbl["p2"];

    -- 参数校验

    -- 约定{pvproomid:1,max_no:2,p1:1,p2:2}{hp:100,p1:1}{hp:100,p2:2},消息发送者为p1,对手为p2
    local ctx_other;

    remote.request("services.battle_pvp", "request_verify_read", msg1, function(msg2)

        local tbl_battle_in_redis = json.multi_decode("{" .. msg2 .. "}");
        local orgin_read = msg2;

        print("1111111111111111111111111111");


        if tbl_battle_in_redis[1]["ret"] == "error" then
            cb("ret:error,msg:房间数据错误,"); return;
        else


            print("2222222222222222222222");


            if tbl["pvproom_id"] ~= tbl_battle_in_redis[1]["pvproom_id"] or tbl["p1"] ~= tbl_battle_in_redis[1]["p1"] or
                tbl["p2"] ~= tbl_battle_in_redis[1]["p2"] then
                -- todo
                -- 如果房间信息有误则通知两个玩家游戏结果校验失败


                remote.request_client(ctx, "Friends", "BattlePVP", "ret:ok,msg:verifyError,", function(msg3)

                end );

                remote.request_client(ctx_other, "Friends", "BattlePVP", "ret:ok,msg:verifyError,", function(msg3)

                end );

            else


                print("333333333333333333333333333");


                if tbl_battle_in_redis[2] == nil then
                    -- 如果是一号玩家


                    print("444444444444444444444");


                    local msg_to_write = "{" .. orgin_read .. "}{" .. "h1:" .. tbl["h1"] .. "," .. "p1:" .. tbl["p1"] .. "," .. "}" .. "{" .. "h2:" .. tbl["h2"] .. "," .. "p2:" .. tbl["p2"] .. "," .. "}";


                    print("6555555555555555555555555");


                    remote.request("services.battle_pvp", "request_verify_write", msg_to_write, function(msg4)

                        print("77777777777777777777777");


                        local kvv = json.decode(msg4);
                        if kvv["ret"] == "ok" then
                            -- 已经写入redis
                            cb("ret:ok,msg:写入成功,");
                        else
                            cb("ret:ok,msg:写入失败,");
                            -- 写入redis失败
                        end

                    end )

                    print("66666666666666666666666666");


                elseif tbl_battle_in_redis[2] ~= nil and tbl_battle_in_redis[3] ~= nil then
                    -- todo josn.encode(tbl_battle_in_redis[2]) == ""
                    -- 如果是二号玩家

                    print("999999999999999999999");








                    if tbl["h1"] == tbl_battle_in_redis[2]["h1"] and tbl["h2"] == tbl_battle_in_redis[3]["h2"] then

                        print("11111110000000000000000000000");


                        -- 发给客户服务器发送房间信息
                        local room_info = "pvproom_id:" .. pvproom_id .. "," .. "p1:" .. p1 .. "," .. "p2:" .. p2 .. ",";
                        remote.request_client_server(room_info, function(msgg)
                            print("11111110000000000000000000000           " .. msgg
                            );

                            local kvv = json.decode(msgg);

                            if tbl_battle_in_redis[2]["h1"] == kvv["h1"] and tbl_battle_in_redis[3]["h2"] == kvv["h2"] then

                                print("22222222222223333333333333333333");


                                remote.request_client(ctx, "BattlePVP", "PushResult", "ret:ok,msg:verifyOk,", function(msg6)
                                    -- Friend?
                                end );

                                remote.request_client(ctx_other, "BattlePVP", "PushResult", "ret:ok,msg:verifyOk,", function(msg6)
                                    -- Friend?
                                end );
                            else
                                remote.request_client(ctx, "BattlePVP", "PushResult", "ret:ok,msg:verifyError,", function(msg6)
                                    -- Friend?
                                end );

                                remote.request_client(ctx_other, "BattlePVP", "PushResult", "ret:ok,msg:verifyError,", function(msg6)
                                    -- Friend?
                                end );
                            end
                        end )

                    else



                        print("888888888888888888888888888");


                        remote.request_client(ctx, "BattlePVP", "PushResult", "ret:ok,msg:verifyError,", function(msg6)
                            -- Friend?
                        end );

                        remote.request_client(ctx_other, "BattlePVP", "PushResult", "ret:ok,msg:verifyError,", function(msg6)
                            -- Friend?
                        end );

                    end
                end


            end

        end
    end );
end


return t;