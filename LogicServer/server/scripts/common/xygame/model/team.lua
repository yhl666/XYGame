
--[[

* Author: caoshanshan
* Email: me@dreamyouxi.com
队伍信息 只保存no 信息 角色信息用其他途径查找
]]
local model = require("model.model");



local t = class("team", model:new());


function t:ctor()

    self.captain = 0;
    -- 队长no
    self.no = 0;
    -- 队伍id
    self.other = 0;
    -- 另外一个玩家no

    self.captain_name = "1";
    self.other_name = "2";
end

function t:is_full()
    if self.no ~= 0 and self.other ~= 0 then
        return true;
    end
    return false;
end
-- 加入玩家
function t:add_player(no)

end
-- 玩家离开
function t:remove_player(no)


end

-- 改变技能组
function t:change_skill(no)


end

-- 改变状态
function t:change_state(no)


end

function t.create()

    return t:new();

end
return t;