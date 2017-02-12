
--[[

* Author: caoshanshan
* Email: me@dreamyouxi.com

]]

local remote = require("xygame.base.remote");

local t = { }

room_global_table = { };

global_players_timeout_ctx = { };

local function check_alive_player()

    for k, v in pairs(global_players_timeout_ctx) do
        table.remove(global_players_ctx, k);

    end
    ---  print("clear all disconnect play ok");
    global_players_timeout_ctx = { };

end


local function notify_other(ctx, service, method, msg, cb)

    for k, v in pairs(global_players_ctx) do

        if ctx:get_game_clt_id() ~= v:get_game_clt_id() then
            -- 广播给其他玩家
            remote.request_client(v, service, method, msg, function(msg)
                if msg == "timeout" then
                    -- 通知失败
                    print("notify other failed");
                    table.insert(global_players_timeout_ctx, ctx);

                end

            end )
        end
    end

    setTimeout(1, check_alive_player);
end


function t.new_position(ctx, msg, cb)
    cb("");
    print("new position " .. msg);

    notify_other(ctx,"Room", "NewPosition", msg);
end
 

 

function t.ckeck_alive(ctx, msg, cb)
    cb("");
    print("check alive   " .. msg);

    notify_other(ctx,"Room", "CheckAlive", msg);
end
 

return t;