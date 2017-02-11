

-- 服务器内部服务例子
-- 用于支持base服务器的RPC请求
-- 在这里示例一个redis数据库服务
local t = { };

-- [Comment]
-- 返回value
-- msg 参数 简单json
-- cb 需要响应时的回调 参数为简单json
function t.getValue(msg, cb)

    print(msg);
    msg = json.decode(msg);
    local name = msg["name"];
    cb("pwd:123,");
end

-- [Comment]
-- 设置value
function t.setValue(key, cb)

end




return t;