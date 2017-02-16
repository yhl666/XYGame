
--[[
provide a  common code to transfer json and class
* Author: caoshanshan
* Email: me@dreamyouxi.com

only string an number can  auto to json
]]
 
local t = class("model");


function t:ctor()


end


function t:to_json()
    local ret = "";

    for k, v in pairs(self) do
        local t_v = type(v);

        if type(k) == "string" and(t_v == "string" or t_v == "number") then
            ret = ret .. k .. ":" .. tostring(v) .. ",";
        end
    end

    return ret;
end


function t:set_json(str)
    local kv = json.decode(str);
    for k, v in pairs(kv) do
        self[k] = v;
    end
end



return t;