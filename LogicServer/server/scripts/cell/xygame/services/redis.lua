-- region *.lua
-- Date
-- 此文件由[BabeLua]插件自动生成



-- endregion


local t = { };

-- [Comment]
-- 返回value
function t.getValue(msg, cb)

    print(msg);
    msg = json.decode(msg);
    local name = msg["name"];
    cb("pwd:123,");
end

-- [Comment]
-- 设置value
function t.setValue(key)

end




return t;