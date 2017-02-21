
--[[

* Author: caoshanshan
* Email: me@dreamyouxi.com

]]


local t = { }



local const = require("base.const")


function t.get_login(key)
    return const.REDIS_KEY_LOGIN .. key
end

function t.get_user(key)
    return const.REDIS_KEY_USER .. key;
end


function t.get_friends(key)
    return const.REDIS_KEY_FRIENDS .. key;
end

function t.get_pvproom(key)
    return const.REDIS_KEY_PVPROOM .. key;
end

return t;