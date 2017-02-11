--[[
Rpc request handler.
Author: Jing Qing  & caoshanshan
]]

local log = require("log"):new("rpc_request_handler")

local M = { }

-- ctx is CRpcCallContext.
function M.handle(ctx, service_name, method_name, req_content)
    require("xygame.base.services_handler").handle(ctx, service_name, method_name, req_content);
end  -- handle()

function M.register_service(name, service)

end  

return M
