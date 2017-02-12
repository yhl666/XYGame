
--[[
公共模块
* Author: caoshanshan
* Email: me@dreamyouxi.com

]]
-----------------------------------pack----

local pb = require("protobuf")

-- [Comment]
-- 为简单json  和protobuf的工具函数，协议转换需要用，但是服务层不需要调用该模块
pack = { }
-- [Comment]
-- json string to pb
function pack.encode(msg)
    return pb.encode("rpc.EnterRoomMsg", { peer_name = msg });
end

-- [Commont]
-- pb to json string
function pack.decode(content)
    return(pb.decode("rpc.EnterRoomMsg", content))["peer_name"];
end



--------------------------------------use ful global function
global_timer = c_timer_queue.CTimerQueue();
-- [Comment]
-- 延迟执行cb函数
-- second 单位秒
-- cb 无参回调
function setTimeout(second, cb)
    global_timer:insert_single_from_now(second, cb);
end












