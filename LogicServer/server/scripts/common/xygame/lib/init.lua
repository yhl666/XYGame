
--[[
初始化一些模块等
* Author: caoshanshan
* Email: me@dreamyouxi.com

---------------------------------------------
API说明：
    pack：为简单json  和protobuf的工具函数，协议转换需要用，但是服务层不需要调用该模块


]]


require("xygame.lib.extern");
require("xygame.lib.json");

local pb = require("protobuf")

pack = { }
-- json string to pb
function pack.encode(msg)
    return pb.encode("rpc.EnterRoomMsg", { peer_name = msg });
end

-- pb to json string
function pack.decode(content)
    return(pb.decode("rpc.EnterRoomMsg", content))["peer_name"];
end




print("lib init ok");