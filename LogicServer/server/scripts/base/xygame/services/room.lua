
--[[

* Author: caoshanshan
* Email: me@dreamyouxi.com

]]

local log = require("log"):new("room")
local remote = require("base.remote");
local hero = require("model.base_hero")
local t = { }


global_base_heros = { };


local function notify_other(ctx, service, method, msg)

    for k, v in ipairs(global_base_heros) do

        if ctx:get_game_clt_id() ~= v.ctx:get_game_clt_id() then
            -- 广播给其他玩家
            remote.request_client(v.ctx, service, method, msg, function(msg)
                if msg == "timeout" then
                    -- 通知失败
                    print("notify other failed hero:" .. v.no);
                end

            end )
        end
    end
end



local function notify_all(ctx, service, method, msg)

    for k, v in ipairs(global_base_heros) do
        -- 广播给玩家
        remote.request_client(v.ctx, service, method, msg, function(msg)
            if msg == "timeout" then
                -- 通知失败
                print("notify other failed hero:" .. v.no);
            end

        end )

    end
end


local function on_disconnected(rpc_clt_id)

    for k, v in pairs(global_base_heros) do
        if v.ctx:get_rpc_clt_id() == rpc_clt_id then
            table.remove(global_base_heros, k);
            print(" hero disconnected no=" .. v.no);

            print("  alive hero   ------list-------");

            for k, v in pairs(global_base_heros) do
                print("hero alive : " .. "  " .. k .. "  " .. v.no);
            end

            print(" alive hero    -------end------");

            notify_all(v.ctx, "Room", "LeaveRoom", "no:" .. v.no .. ",");
            return;
        end
    end

end

function t.new_position(ctx, msg, cb)
    cb("");
    notify_other(ctx, "Room", "NewPosition", msg);
end
 
function t.enter_room(ctx, msg, cb)
    local kv = json.decode(msg);

    remote.request_client(ctx, "Room", "SelfEnterRoom", "no:" .. kv["no"] .. ",name:" .. kv["name"] .. ",", function(msgg)

        if msgg == "" then return end;
        --- 响应失败 忽略

        -- 响应成功后 添加到table里面

       
        notify_all(ctx, "Room", "EnterRoom", msg);
         table.insert(global_base_heros, hero.create(ctx, kv["no"]));


    end );
    cb("ret:ok");
end
 

function t.ckeck_alive(ctx, msg, cb)
    cb("");
    notify_other(ctx, "Room", "CheckAlive", msg);
end
 

 



require("event_handler").register("ClientDisconnected", on_disconnected);

return t;