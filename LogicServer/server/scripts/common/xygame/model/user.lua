
--[[

* Author: caoshanshan
* Email: me@dreamyouxi.com

]]
local model = require("model.model");



local t = class("user", model:new());


function t:ctor()

    self.no = 0;
    -- id
    self.name = "111";
    -- �ǳ�
    self.level = 50;
    -- �ȼ�
    self.type = 0;
    -- ���ͣ�
    self.last_login_timestamp = 0;
    -- �ϴε�¼ʱ���

end

 

local test = t:new();
local test11 = t:new();
test.name = "333";



print((test:to_json()));
test11:set_json(test:to_json())
print(test11:to_json());


return t;