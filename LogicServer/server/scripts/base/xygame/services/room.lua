
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



function t.new_position(ctx, msg, cb)
    cb("");
    print("new position " .. msg);
    local kv = json.decode(msg);

    for k, v in pairs(global_players_ctx) do

        if ctx:get_game_clt_id() ~= v:get_game_clt_id() then
            -- 广播给其他玩家
            remote.request_client(v, "Room", "NewPosition", msg, function(msg)
                if msg == "timeout" then
                    -- 通知失败
                    print("notifu other faild");
                    table.insert(global_players_timeout_ctx, ctx);

                else
                    -- 该玩家通知成功

                end

            end )
        end
    end


    setTimeout(1, check_alive_player);
end
 



return t;