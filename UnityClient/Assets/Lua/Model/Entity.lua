--主入口函数。从这里开始lua逻辑

local t=class("Model")
local model = require("Model.Model");
function t:ctor()
	self.base=model.new();
 
	setmetatable(self,self.base);
end



function t:UpdateMS()

print(" ms ms 1111");
end



return t;