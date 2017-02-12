
--[[
远程调用接口
* Author: caoshanshan
* Email: me@dreamyouxi.com
----------------------------------------------------------------------------
服务器需要发起RPC调用的时候通过此模块来进行
#在这里base 和cell概念有点小变动
#base服务器和客户端交互的服务器
#cell服为base提供服务器内部服务，比如数据库服务等
--TODO:在C++层 也添加这种概念 使得能在C++层协同处理服务
--------------------------------------------------------------------------------
功能
1.向cell服务器发起内部RPC请求
2.向游戏客户端发起RPC请求

]]


local log = require("log"):new("remote")

local t = { }

-- [Comment]
-- 向cell服务器发起RPC请求
-- services #string 服务类型
-- method #string 函数名字
-- json #string 参数,简单json
-- cb #function 回调，参数为简单数据类型
function t.request(services, method, json, cb)
    log:debug("send a remote services request to cell server");

    local called = false;
    c_rpc.request_svr("3", "remote." .. services, method, pack.encode(json), function(msg)
        if called == true then return end
        called = true
        cb(pack.decode(msg));
    end );

    setTimeout(1, function()
        -- 2秒超时
        if called == false then
            cb("timeout");
            log:error("call cell rpc timeout " .. services .. "." .. method);
            -- timeout
        else
            cb("");
        end
    end )



end


-- [Comment]
-- 向游戏客户端发起RPC请求
function t.request_client(ctx, services, method, json, cb)
    log:debug("send a client services request to cell server");

    local called = false;
    c_rpc.request(ctx:get_rpc_clt_id(), services, method, pack.encode(json), function(msg)
        if called == true then return end
        called = true
        cb(pack.decode(msg));
    end );

    setTimeout(1, function()
        -- 5秒超时
        if called == false then
            cb("timeout");
            log:error("call game client rpc timeout " .. services .. "." .. method);
        end
    end )

end

log:debug("load");

return t;