
--[[

* Author: caoshanshan
* Email: me@dreamyouxi.com

]]


local t = { }

local log = require("log"):new("friends")
local remote = require("base.remote");

-- 删除好友
function t.remove(ctx, msg, cb)
    -- 发起cell服务器的远程调用，请求 remove
    remote.request("services.friends", "remove", msg, function(msg)
        if msg == "timeout" then
            cb("ret:error,msg:timeout,");
            return;
        end
        cb(msg);
    end );
end

-- 添加好友
function t.add(ctx, msg, cb)

    -- 发起cell服务器的远程调用，请求 add
    remote.request("services.friends", "add", msg, function(msg)
        if msg == "timeout" then
            cb("ret:error,msg:timeout,");
            return;
        end
        cb(msg);
    end );



end


function t.add_by_name(ctx, msg, cb)
    local kvv = json.decode(msg);

    -- 发起cell服务器的远程调用，请求 add
    remote.request("services.friends", "add_by_name", msg, function(msg1)

        if msg1 == "timeout" then
            cb("ret:error,msg:timeout,");
            return;
        end

        local kv = json.decode(msg1);

        if kv["ret"] == "ok" then


            -- 反向添加好友
            remote.request("services.friends", "add_by_no", "no:" .. kv["no"] .. ",who:" .. kvv["no"] .. ",", function(msg)

                if msg == "timeout" then
                    cb("ret:error,msg:timeout,");
                    -- 反向添加失败 待删除
                    return;
                end
                cb(msg1);
                -- 反向添加成功

            end );

        else
            cb(msg1);
        end

    end );

end

function t.add_by_no(ctx, msg, cb)

    -- 发起cell服务器的远程调用，请求 add
    remote.request("services.friends", "add_by_no", msg, function(msg1)

        if msg1 == "timeout" then
            cb("ret:error,msg:timeout,");
            return;
        end




        local kv = json.decode(msg1);

        local kvv = json.decode(msg);

        if kv["ret"] == "ok" then


            -- 反向添加好友
            remote.request("services.friends", "add_by_no", "no:" .. kv["no"] .. ",who:" .. kvv["no"] .. ",", function(msg)

                if msg == "timeout" then
                    cb("ret:error,msg:timeout,");
                    -- 反向添加失败 待删除
                    return;
                end
                cb(msg1);
                -- 反向添加成功

            end );

        else
            cb(msg1);
        end

    end );

end



-- 发消息
function t.send(ctx, msg, cb)
    local kv = json.decode(msg);
    local no = kv["no"];
    local who = kv["who"];
    local content = kv["msg"];

    -- 先查询是否有该好友，
    remote.request_local("services.friends", "query", "no:" .. no .. ",who:" .. who .. ",", function(res)

        if res == "" then
            cb("ret:error,msg:" .. " 你们还不是好友关系,");
        else
            local hero = global_hero_list:get_hero_by_no(no);

            if hero == nil then
                cb("ret:error,msg:" .. " 你的好友不在线,");
            else
                cb("ret:ok,");
                -- remote.request_client ();



            end
        end

    end );
end



function t.query_all(ctx, msg, cb)

    -- 发起cell服务器的远程调用，请求 query
    remote.request("services.friends", "query_all", msg, function(msg)

        if msg == "timeout" then
            cb("ret:error,msg:timeout,");
            return;
        end
        cb(msg);
    end );


end

-- 查询某个好友
function t.query_one(ctx, msg, cb)

    -- 发起cell服务的远程调用
    remote.request("services.friends", "query_one", msg, function(msg)

        if "timeout" == msg then
            cb("ret:error,msg:timeout,");
            return;
        end
        cb(msg);
    end );
end


function t.delete_by_no(ctx, msg, cb)
    -- 发起cell服务器的远程调用，请求 delete
    remote.request("services.friends", "delete_by_no", msg, function(msg1)

        if msg1 == "timeout" then
            cb("ret:error,msg:timeout,");
            return;
        end
        ---   cb(msg);

        local kv = json.decode(msg1);

        local kvv = json.decode(msg);

        if kv["ret"] == "ok" then


            -- 反向删除好友
            remote.request("services.friends", "delete_by_no", "no:" .. kvv["who"] .. ",who:" .. kvv["no"] .. ",", function(msg)

                if msg == "timeout" then
                    cb("ret:error,msg:timeout,");
                    -- 反向删除失败 待删除
                    return;
                end
                cb(msg1);
                -- 反向删除成功

            end );

        else
            cb(msg1);
        end


    end );
end


return t;