
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
    -- �ǳ�
    self.level = "1";
    -- �ȼ�
    self.type = "0";
    -- ���ͣ�
    self.time = "0";
    -- �ϴε�¼ʱ���

end

 
 function t.create()

  return t:new();

end
return t;