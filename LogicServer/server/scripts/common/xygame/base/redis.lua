
--[[
redis helper modele
* Author: caoshanshan
* Email: me@dreamyouxi.com

]]


local t = { }



--[Comment]
-- ����Ϊ""��ʾʧ��
-- "ok" ��ʾ�ɹ�
function t.set(key, value, cb)

    c_redis.set(key, value, function(msg)
        if msg == "OK" then
            cb("ok");
        else
            cb("");
        end

    end )
end



--[Comment]
-- ����Ϊ""��ʾʧ��
-- ������ʾ ��ȡ��ֵ ��ʾ�ɹ�
function t.get(key, cb)

    c_redis.get(key, function(msg)
        if msg == "" then
            cb("");
        else
            cb(msg);
        end

    end )
end





return t;