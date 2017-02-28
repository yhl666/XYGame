
--[[

* Author: caoshanshan
* Email: me@dreamyouxi.com
should use DAO to instead ?
]]
local model = require("model.equip");



local t = class("equip", model:new());


function t:ctor()

    self.id = "0"
    -- 编号
    self.no = "0";
    -- 全服ID
    self.ownner = "0"
    -- 拥有真no
    self.level = 1;
    -- 等级
    self.exp = 0;
    -- 经验

end

 


function t:add_exp(exp)
    self.exp = self.exp + tonumber(exp);
    local max_exp = self:get_current_max_exp(self.level);
    if self.exp >= max_exp then

        self.level = self.level + 1;
        self.exp = max_exp - self.exp;
    end

end


function t:get_current_max_exp(level)

end



function t.create()

    return t:new();

end
return t;