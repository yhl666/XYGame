
--[[

* Author: caoshanshan
* Email: me@dreamyouxi.com
should use DAO to instead ?
]]
local model = require("model.model");



local t = class("user", model:new());


function t:ctor()

    self.no = "0";
    -- id
    self.name = "111";
    -- 昵称
    self.level = "1";
    -- 等级
    self.type = "0";
    -- 类型，
    self.time = "0";
    -- 上次登录时间戳

end

 
 function t.create()

  return t:new();

end
return t;