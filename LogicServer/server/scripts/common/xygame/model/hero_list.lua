--[[

* Author: caoshanshan
* Email: me@dreamyouxi.com

房间玩家的容器
]]

local t = class("hero_list");


function t:ctor()
    self.list = { };
    self.size = 0;
end


function t:add(base_hero)

    table.insert(self.list, base_hero);

    self.size = self.size + 1;
end


function t:remove(base_hero)
    if self.size == 0 then return end;

    table.remove(self.list, base_hero);
    self.size = self.size - 1;
end


function t:clear()
    self.size = 0;
    self.list = { };
end

function t:current_size()
    return self.size;

end

function t:foreach(cb)
    local dirty_list = { };
    for k, v in pairs(self.list) do
        cb(k, v);
        if v.is_dirty == true then
            table.insert(dirty_list, k);

        end
    end

    for k, v in ipairs(dirty_list) do

        self:remove(v);
    end

end



function t:remove_by_rpc_cli_id(id)

    for k, v in pairs(self.list) do
        if v.ctx:get_rpc_clt_id() == id then
            v.is_dirty = true; --标识玩家是否在线
            self:remove(k);
            return v;
        end

    end

    return nil;
end

function t:get_hero_by_no(no)

    for k, v in pairs(self.list) do
        if v.no == no then
            return v;
        end
    end
    return nil;
end


return t;