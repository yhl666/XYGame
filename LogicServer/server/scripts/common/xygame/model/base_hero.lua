--[[

* Author: caoshanshan
* Email: me@dreamyouxi.com

Town 下的base hero 模型

]]

local t = { }

function t.create(ctx, no, name)
    local ret = { };
    ret.ctx = ctx;
    ret.is_dirty = false;
    ret.no = no;
    ret.name = name;

    return ret;
end


return t;