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
                cb("ret,error,msg:你没有这件装备");
                return;

            end


            local model_equip = equip.create();
            model_equip:add_exp(exp);

            kvs[key] = model_equip:to_json();

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

 
return t;
