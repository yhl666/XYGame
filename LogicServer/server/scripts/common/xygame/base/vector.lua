--[[

* Author: caoshanshan
* Email: me@dreamyouxi.com
 容器
]]

local t = class("vector");


function t:ctor()
    self.list = { };
    self.size = 0;
end


function t:add(obj)

    table.insert(self.list, obj);
    self.size = self.size + 1;
end

function t:remove(obj)
    if self.size == 0 then return end;

    for k, v in pairs(self.list) do
        if v == obj then
            table.remove(self.list, k);
            self.size = self.size - 1;
            return;
        end
    end
end


function t:clear()
    self.size = 0;
    self.list = { };
end

function t:current_size()
    return self.size;

end

-- cb 返回false 将会终止循环
function t:foreach(cb)
    local dirty_list = { };
    for k, v in pairs(self.list) do
        local ret = cb(k, v);
        if ret == false then
            return;
        end
        if v.is_dirty == true then
            --   table.insert(dirty_list, k);
        end
    end

    for k, v in ipairs(dirty_list) do

        self:remove(v);
    end

end



return t;