
--[[
派发rpc服务
* Author: caoshanshan
* Email: me@dreamyouxi.com

-----------------------------------------------------------------------------
在  rpc_request_handler.lua 的handle函数改为以下代码

function M.handle(ctx, service_name, method_name, req_content)
    require("xygame.base.services_handler").handle(ctx, service_name, method_name, req_content);
end  -- handle()

达到接管handle的目的
------------------------------------------------------------------------------
功能：
1.自动找到服务目录并且加载，首次加载会自动添加到热更新模块
2.自动识别是base服务还是cell服务（来自客户端 或者base服务器）
3.将传输协议从protobuf改为 类型为string 的简单json
4.将应答等交互方式优化为 闭包处理 .使用示例在各个服务下的example.lua
5.转换base 和cell服务器的概念 详见remote.lua
------------------------------------------------------------------------------

]]

 
 
local t = { };

local service_map = { }
local log = require("log"):new("services_handler")

--[Comment]
-- C++调用，无需修改 访问 此文件内容 
function t.handle(ctx, service_name, method_name, req_content)

    local isRemote = string.find(service_name, "remote.") ~= nil;

    if isRemote then
        service_name = string.sub(service_name, 8, -1);
        service_name = "cell.xygame." .. service_name;
        -- 服务器本机远程服务
    else
        service_name = "base.xygame." .. service_name;
        -- 游戏客户端服务
    end


    local service = service_map[service_name]
    if service == nil then
        -- is file exist
        local f = G_LUA_ROOTPATH .. "/" ..(string.gsub(service_name, "[\\.]", "/")) .. ".lua";
        local file, err = io.open(f, "r+");
        if err ~= nil then
            if isRemote then
                log:warn("No such remote service: " .. service_name);
            else
                log:warn("No such service: " .. service_name);
            end
            c_rpc.reply_to(ctx, pack.encode(""));
            return;
        end



        -- 如果模块存在，那么自动添加热更新
        if require(service_name).enable_hotfix ~= false then
            require("hotfix_helper").add_module_name(service_name)
        end
        file:close();
    end

    service = require(service_name);
    service_map[service_name] = service;
    local methodFunc = service[method_name];

    if methodFunc == nil then
        log:warn("No such function: " .. service_name .. "." .. method_name);
        c_rpc.reply_to(ctx, pack.encode(""));
        return;
    end


    if isRemote then
        -- 远程cell 服务不需要ctx 因为远程服务 不能访问客户端 只需要处理cb即可
        methodFunc(pack.decode(req_content), function(msg)
            c_rpc.reply_to(ctx, pack.encode(msg));

        end );
    else
        methodFunc(ctx, pack.decode(req_content), function(msg)
            c_rpc.reply_to(ctx, pack.encode(msg));
        end );
    end

end

return t;