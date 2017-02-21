
--[[

* Author:  sutao
* Email:   sutao@ztgame.com


--{no:1,no_target:2,}no我的id no_target 我要战的id
--request_pvp_v1(ctx, msg, cb)



--{hp:50,no:1,}{hp:0,no:0,}
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
                        remote.request_client(ctx_2, "Friends", "RecvPVP", user1 ..msg5, function(msg)

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

return t;