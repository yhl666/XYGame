
--[[

* Author: caoshanshan
* Email: me@dreamyouxi.com

]]


local t = { }



local const = require("base.const")


function t.get_login(key)
    return const.REDIS_KEY_LOGIN .. key
end


return t;