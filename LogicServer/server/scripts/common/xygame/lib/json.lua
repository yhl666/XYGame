
--[[
xygame传输数据协议的 简单json

* Author: caoshanshan
* Email: me@dreamyouxi.com
------------------------------------------------------
全部是以key value 存在的 键值对
示例 "name:css,pwd:123,"  注意有几个kv 就有几个逗号
note：因为没有{}不能递归包含，所有key 都是平行关系

]]



require("string");

json = { }

--[Comment]
-- 把 json解析为table
function json.decode(str)
    local ret = { }

    while true do
        local index = string.find(str, ",");
        if index == nil then
            break;
        end
        local sub = string.sub(str, 0, index - 1);
        if sub == nil then
            break;
        end
        str = string.sub(str, index + 1, -1);


        local i = string.find(sub, ":");
        local k = string.sub(sub, 0, i - 1);
        local v = string.sub(sub, i + 1, -1);

        ret[k] = v;
    end

    return ret;
end

--[Comment]
-- 吧table 解析为 json
function json.encode(t)
    local ret = "";
    for k, v in pairs(t) do
        ret = ret .. k .. ":" .. v .. ",";
    end
    return ret;
end