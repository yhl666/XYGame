
require("xygame.lib.extern");
require("xygame.lib.json");

local pb = require("protobuf")

pack = { }
-- json string to pb
function pack.encode(msg)
    local t = { peer_name = msg };
    return pb.encode("rpc.EnterRoomMsg", t);
end

-- pb to json string
function pack.decode(content)
    local t = pb.decode("rpc.EnterRoomMsg", content);
    return t["peer_name"];
end

print("lib init ok");