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

    self.list[base_hero.ctx] = base_hero;
    self.size = self.size + 1;
end


function t:remove(base_hero)
    if self.size == 0 then return end;

    table.remove(self.list, base_hero.ctx);
    self.size = self.size - 1;
end


function t:clear()
    self.size = 0;
    self.list = { };
end

function t:size()
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


return t;