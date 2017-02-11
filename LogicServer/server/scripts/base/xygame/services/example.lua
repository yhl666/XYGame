local t = { }

local log = require("log"):new("login")
local remote = require("xygame.base.remote");-- Զ�̷���
-- �Ϳͻ��˽����ķ���ʵ��

-- [Comment]
-- ctx �����ģ���Ϊ����Ҫ����ͻ��˵���������Ҫ����ctx
-- msg ���� �Ǽ򵥵�json��ʽȫΪ�ַ���
-- cb ��Ҫ��Ӧ��ʱ��Ļص�����������Ϊ��json��ʽ
function t.login(ctx, msg, cb)
    local kv = json.decode(msg);

    -- ����cell��������Զ�̵��ã����� ���ݿ����
    remote.request("services.redis", "getValue", "name:" .. kv["name"] .. ",", function(msg)

        local m = json.decode(msg);

        if kv["pwd"] == m["pwd"] then
            cb("res:ok,");
        else
            cb("res:error,");
        end

    end );

    -- ����Ϸ�ͻ��˷���rpc����
    remote.request_client(ctx, "rpctest41", "func", "name:3,", function(msg)
        print(msg .. "  recv respone");

    end );

end

function t.logout(msg, cb)

end

return t;