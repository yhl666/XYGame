
--[[

* Author: caoshanshan
* Email: me@dreamyouxi.com

]]

local log = require("log"):new("room")
local remote = require("base.remote");
local hero = require("model.base_hero")
local hero_list = require("model.hero_list")
local user = require("model.user")
local t = { }

t.enable_hotfix = false;  -- 禁止hot fix

global_hero_list = hero_list.new();

function room_notify_other(ctx, service, method, msg)

    global_hero_list:foreach( function(k, v)

        if v.ctx:get_game_clt_id():equals(ctx:get_game_clt_id()) == false then
            -- 广播给其他玩家
            remote.request_client(v.ctx, service, method, msg, function(msg)
                if msg == "timeout" then
                    -- 通知失败
                    v.is_dirty = true;

                else
                end

            end )
        end
    end );

end



function room_notify_all(service, method, msg)

    global_hero_list:foreach( function(k, v)
        remote.request_client(v.ctx, service, method, msg, function(msg)
            if msg == "timeout" then
                -- 通知失败
                v.is_dirty = true;
            end
        end )

    end );
end


local function on_disconnected(rpc_clt_id)
    local hero = global_hero_list:remove_by_rpc_cli_id(rpc_clt_id);
    if hero == nil then
        -- 表示不是客户端 而是cell 服务器断开了连接
        log:error(" Cell Server id= " .. rpc_clt_id .. " has Disconnected");
        return;
    end
    room_notify_all("Room", "LeaveRoom", "no:" .. hero.user.no .. ",");
end


function t.new_position(ctx, msg, cb)
    cb("");
    room_notify_other(ctx, "Room", "NewPosition", msg);
end
 
function t.enter_room(ctx, msg, cb)
    local kv = json.decode(msg);

    remote.request_client(ctx, "Room", "SelfEnterRoom", msg, function(msgg)

        if msgg == "" then return end;
        --- 响应失败 忽略

        -- 响应成功后 添加到table里面

        room_notify_all("Room", "EnterRoom", msg);

        print(msg);
        local user1 = user.create();

        user1:set_json(msg);
        -- 暂时以客户端传来的为准

        global_hero_list:add(hero.create(ctx, user1));

    end );
    cb("ret:ok,");
end
 

function t.ckeck_alive(ctx, msg, cb)
    cb("");
    room_notify_other(ctx, "Room", "CheckAlive", msg);
end
 

 



require("event_handler").register("ClientDisconnected", on_disconnected);

return t;