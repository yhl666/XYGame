
--[[

* Author: caoshanshan
* Email: me@dreamyouxi.com

]]

local remote = require("xygame.base.remote");

local t = { }




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
 

 

function t.ckeck_alive(ctx, msg, cb)
    cb("");
    notify_other(ctx, "Room", "CheckAlive", msg);
end
 

 



require("event_handler").register("ClientDisconnected", on_disconnected);

return t;