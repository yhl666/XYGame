--[[
Rpc request handler.
Author: Jin Qing (http://blog.csdn.net/jq0123)
]]

local M = { }

local service_map = { }
local log = require("log"):new("rpc_request_handler")
--[[
-- ctx is CRpcCallContext.
function M.handle(ctx, service_name, method_name, req_content)
	log:debug("handle(%s.%s, req(len=%u))", service_name, method_name, #req_content)
	service = service_map[service_name]
	if not service then
		log:warn("No such service: " .. service_name)
		return
	end
	method = service[method_name]
	if not method then
		log:warn("No such method: " .. service_name .. "." .. method_name)
		return
	end

	method(ctx, req_content)
end  -- handle()


function M.register_service(name, service)
	log:debug("register service: name=%s, service=%s" , name, service)
	service_map[name] = service
end  -- register_service

log:debug("Loaded.")
return M



]]--





local M = { }

local service_map = { }
local log = require("log"):new("rpc_request_handler")

-- ctx is CRpcCallContext.
function M.handle(ctx, service_name, method_name, req_content)
    log:debug("handle(%s.%s, req(len=%u))", service_name, method_name, #req_content)
    require("xygame.base.services_handler").handle(ctx, service_name, method_name, req_content);
end  -- handle()

 
function M.register_service(name, service)

end  -- register_service
log:debug("Loaded.")
return M
