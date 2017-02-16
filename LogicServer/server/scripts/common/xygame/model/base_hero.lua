--[[

* Author: caoshanshan
* Email: me@dreamyouxi.com

Town 下的base hero 模型

]]

local t = { }

function t.create(ctx, user)
    local ret = { };
    ret.ctx = ctx;
    ret.is_dirty = false;
    ret.user= user;

    return ret;
end


return t;