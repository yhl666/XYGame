
--[[

* Author: caoshanshan
* Email: me@dreamyouxi.com

]]

local remote = require("xygame.base.remote");

local t = { }

room_global_table = { };




function t.new_position(ctx, msg, cb)
    print("new position " .. msg);
    local kv = json.decode(msg);

    for k, v in pairs(global_players_ctx) do

        if ctx:get_rpc_clt_id() ~= v:get_rpc_clt_id() then
            -- 广播给其他玩家
            remote.request_client(v, "Room", "NewPosition", msg, function(msg)
                if msg == "" then
                    -- 该玩家已离开房间 移除列表


                    remote.request_client(v, "Room", "LeaveRoom", "no:" .. kv["no"] .. ",", function(msg)
                        if msg == "" then
                            --  通知失败

                            return;
                        end
                    end )


                    return;
                end
            end )
        end
    end

end
 



return t;