
--[[

* Author: caoshanshan
* Email: me@dreamyouxi.com
������Ϣ ֻ����no ��Ϣ ��ɫ��Ϣ������;������
]]
local model = require("model.model");



local t = class("team", model:new());


function t:ctor()

    self.captain = 0;
    -- �ӳ�no
    self.no = 0;
    -- ����id
    self.other = 0;
    -- ����һ�����no

    self.captain_name = "1";
    self.other_name = "2";
end

function t:is_full()
    if self.no ~= 0 and self.other ~= 0 then
        return true;
    end
    return false;
end
-- �������
function t:add_player(no)

end
-- ����뿪
function t:remove_player(no)


end

-- �ı似����
function t:change_skill(no)


end

-- �ı�״̬
function t:change_state(no)


end

function t.create()

    return t:new();

end
return t;