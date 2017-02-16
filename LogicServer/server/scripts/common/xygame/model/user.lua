
--[[

* Author: caoshanshan
* Email: me@dreamyouxi.com
should use DAO to instead ?
]]
local model = require("model.model");



local t = class("user", model:new());


function t:ctor()

    self.no = 0;
    -- id
    self.name = "111";
    -- 昵称
    self.level = 1;
    -- 等级
    self.type = 2;
    -- 类型，
    self.time = 0;
    -- 上次登录时间戳

end

 

local test = t:new();
local test11 = t:new();
test.name = "333";



print((test:to_json()));
test11:set_json(test:to_json())
print(test11:to_json());


return t;