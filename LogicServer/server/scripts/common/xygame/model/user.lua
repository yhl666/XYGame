
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

    self.no_dec = "0";
    -- 穿上的饰品
    self.no_atk = "0";
    -- 穿上的武器
    self.no_def = "0";
    -- 穿上的防具

    self.id_dec = "0";
    -- 穿上的饰品id
    self.id_atk = "0";
    -- 穿上的武器id
    self.id_def = "0";
    -- 穿上的防具id

    self.exp = 0;
    -- 经验

end

 
 function t.create()

  return t:new();

end
return t;