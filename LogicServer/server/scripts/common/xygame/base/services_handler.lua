-- region *.lua
-- Date
-- 此文件由[BabeLua]插件自动生成



-- endregion
require("string");

local t = { };

local service_map = { }
local log = require("log"):new("services_handler")


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
        require("hotfix_helper").add_module_name(service_name);
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

log:debug("load");

return t;