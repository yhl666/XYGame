
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

    self.no_dec = "0";
    -- ���ϵ���Ʒ
    self.no_atk = "0";
    -- ���ϵ�����
    self.no_def = "0";
    -- ���ϵķ���

    self.id_dec = "0";
    -- ���ϵ���Ʒid
    self.id_atk = "0";
    -- ���ϵ�����id
    self.id_def = "0";
    -- ���ϵķ���id

    self.exp = 0;
    -- ����

end

 
 function t.create()

  return t:new();

end
return t;