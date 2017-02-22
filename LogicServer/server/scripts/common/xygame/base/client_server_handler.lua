
--[[
--外部不需要访问该文件接口，请用remote.lua 来访问
* Author: caoshanshan
* Email: me@dreamyouxi.com

]]

local t = { }



local client_room_map = { }


function t.handle_client_server(room_info)

    local kv = json.decode(room_info);
    local key = "pvproom_id:" .. kv["pvproom_id"] .. ",p1:" .. kv["p1"] .. ",p2:" .. kv["p2"] .. ",";

    local cb = client_room_map[key];

    print("key=" .. key);

    cb(room_info);

end


function t.add_client_server(room_info, cb)

    client_room_map[room_info] = cb;
    print("reg " .. room_info .. tostring(client_room_map[tostring(room_info)]));

end


-- "pvproom_id:2,p1:1,p2:2,"
function t.request_client_server(room_info, cb)

    local kv = json.decode(room_info);
    local key = "pvproom_id:" .. kv["pvproom_id"] .. ",p1:" .. kv["p1"] .. ",p2:" .. kv["p2"] .. ",";

    t.add_client_server(key, cb);
    client_server.send(key);

end
return t
