--主入口函数。从这里开始lua逻辑

local t=class("Model")

function t:ctor()
	self.base=LuaModel.new();
	
	self.base:bind(self);
end



function t:UpdateMS()

print(" ms ms ");
end



return t;