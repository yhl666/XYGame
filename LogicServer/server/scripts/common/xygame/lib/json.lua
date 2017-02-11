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
--吧table 解析为 json
function json.encode(t)
    local ret = "";
    for k, v in pairs(t) do
        ret = ret .. k .. ":" .. v .. ",";
    end
    return ret;
end