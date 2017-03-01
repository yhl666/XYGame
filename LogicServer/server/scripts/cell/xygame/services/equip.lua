--[[
* Author:  cao shanshan
* Email:   me@dreamyouxi.com

* Author:  sutao
* Email:   sutao@ztgame.com

]]

local t = { }

local log = require("log"):new("equip")
local remote = require("base.remote");

local redis = require("base.redis");
local redis_key = require("utils.redis_key")
local equip = require("model.equip");

 
function t.add_exp(msg, cb)

    local kv = json.decode(msg);
    local exp = kv["exp"];
    local no = kv["no"];
    local ownner = kv["ownner"];

    redis.get(redis_key.get_equip(ownner), function(msg1)

        --print(msg1);

        if msg1 == "" then
            cb("ret:error,");
        else
            local kvs = json.multi_decode(msg1);
            if kvs == nil then
                cb("ret:error,");
                return;
            end

            local equip_table = nil;
            local key = nil;
            for k, v in ipairs(kvs) do

                if v.no == no then
                    equip_table = v;
                    key = k;
                    break;
                end
            end

            if equip_table == nil then
                cb("ret,error,msg:你没有这件装备,");
                return;

            end

            local model_equip = equip.create();
            model_equip.no = no;
            model_equip:add_exp(exp);

          
            --print(model_equip:to_json());
            kvs[key] =json.decode(model_equip:to_json());
            local data = json.multi_encode(kvs);


            redis.set(redis_key.get_equip(ownner), data, function(msg1)
                if msg1 == "ok" then
                    cb("ret:ok," .. model_equip:to_json());
                else
                    cb("ret:error,msg:数据写入失败,"); return;
                end
            end )


        end
    end )

end
 
-- 产生一件装备
-- ownner 拥有者id，id 装备id
function t.gen_a_random(msg, cb)

    local kv = json.decode(msg);
    local ownner = kv["ownner"];
    local id = kv["id"];

    -- 修改初始化的装备id
    local model_equip = equip.create();
    model_equip.id = id;
    model_equip.ownner = ownner;

   
    redis.exec("incr GLOBAL_EUQIP_MAX_NO", function(msggg)

        print("exec redis cmd: " .. msggg);
        local kvv = json.decode(msggg);
        model_equip.no = kvv["msg"];

        if (kvv["ret"] == "ok") then
            -- 自动增长ID成功

            local msg = "{" .. model_equip:to_json() .. "}";
            redis.get(redis_key.get_equip(ownner), function(msg1)

                msg1 = msg1 .. msg;
                print("ret1" .. msg1);
                redis.set(redis_key.get_equip(ownner), msg1, function(msg2)

                    if msg2 == "ok" then
                        cb("ret:ok," .. model_equip:to_json()); return;
                    end
                    cb("ret:error,");
                end );

            end )
        end

    end )
end


-- 查询该玩家所有装备
-- ownner

-- 返回的msg为：ret:ok,{装备信息}{装备信息}...{}
function t.query_all(msg, cb)

    local kv = json.decode(msg);
    local ownner = kv["ownner"];

    redis.get(redis_key.get_equip(ownner), function(msg1)

        if msg1 == "" then
            cb("");

        else
            cb(msg1);
        end

    end )
end

-- 查询一条装备信息
-- ownner
-- no
function t.query(msg, cb)

    local kv = json.decode(msg)
    local no = kv["no"];
    local ownner = kv["ownner"];
    -- print("no:" .. no);
    -- print("ownner:" .. ownner);


    redis.get(redis_key.get_equip(ownner), function(msg1)

        if msg1 == "" then
            cb("ret:error,");
        else
            local kvv = json.multi_decode(msg1);
            if kvv == nil then
                cb("ret:error,");
                return;
            end

            local equip_table = nil;
            for k, v in ipairs(kvv) do
                if v.no == no then
                    equip_table = v;
                end
            end
            cb("ret:ok," .. json.encode(equip_table));
        end
    end )
end

-- 修改装备属性
-- ownner
-- no
function t.modify(args)

end

return t;
