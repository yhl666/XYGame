
--[[
xygame��������Э��� ��json

* Author: caoshanshan
* Email: me@dreamyouxi.com
------------------------------------------------------
ȫ������key value ���ڵ� ��ֵ��
ʾ�� "name:css,pwd:123,"  ע���м���kv ���м�������
note����Ϊû��{}���ܵݹ����������key ����ƽ�й�ϵ

��ʾ��� 
{ "name:css,pwd:123,"}{ "name:css,pwd:123,"}{ "name:css,pwd:123,"}
]]



require("string");

json = { }

--[Comment]
-- �� json����Ϊtable
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
-- ��table ����Ϊ json
function json.encode(t)
    local ret = "";
    for k, v in pairs(t) do
        ret = ret .. k .. ":" .. v .. ",";
    end
    return ret;
end



-- [Comment]
-- ��table ����Ϊ  ���{}�ϲ���json
function json.multi_encode(t)
    local ret = "";
    for k, v in ipairs(t) do
        ret = ret .. "{" .. json.encode(v) .. "}";
    end
    return ret;
end


--[Comment]
-- �� json����Ϊ int Ϊkey �Ķ��table
function json.multi_decode(str)
    local ret = { };

    local function add_table(what)
        table.insert(ret, what);
    end

    while true do
        local index = string.find(str, "}");
        if index == nil then
            break;
        end
        local sub = string.sub(str, 2, index - 1);
        if sub == nil then
            break;
        end

        print("    " .. sub);

        add_table(json.decode(sub));

        str = string.sub(str, index + 1, -1);
    end

    return ret;
end






-- print (json.multi_encode(json.multi_decode("{name:10,pwd:1254,}{haha:12����5,}")));

