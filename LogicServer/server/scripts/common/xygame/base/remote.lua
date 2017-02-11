-- region *.lua
-- Date
-- 此文件由[BabeLua]插件自动生成



-- endregion

local log = require("log"):new("remote")
local timer = c_timer_queue.CTimerQueue();
local t = { }

-- [Comment]
-- 向cell服务器发起RPC请求
function t.request(services, method, json, cb)
    log:debug("send a remote services request to cell server");

    local called = false;
    c_rpc.request_svr("3", "remote." .. services, method, pack.encode(json), function(msg)
        called = true
        cb(pack.decode(msg));
    end );

    timer:insert_single_from_now(2, function()
        -- 2秒超时
        if called == false then
            cb("timeout");
            log:error("call cell rpc timeout " .. services .. " " .. method);
            -- timeout
        end
    end )



end


-- [Comment]
-- 向游戏客户端发起RPC请求
function t.request_client(ctx, services, method, json, cb)
    log:debug("send a client services request to cell server");

    local called = false;
    c_rpc.request(ctx:get_rpc_clt_id(), services, method, pack.encode(json), function(msg)
        called = true
        cb(msg);
    end );

    timer:insert_single_from_now(5, function()
        -- 5秒超时
        if called == false then
            cb("timeout");
            log:error("call gameclient rpc timeout " .. services .. " " .. method);
        end
    end )

end

log:debug("load");

return t;