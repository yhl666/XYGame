
--[[

* Author: caoshanshan
* Email: me@dreamyouxi.com

]]
local t=class("queue")
--[Comment]
--Queue
--
function t:ctor()
    self.data={}
    self.f=0
    self.e=0
end



--[Comment]
--push value
function t:push(value)
    self.data[self.e]=value
    self.e=self.e+1
end

 

--[Comment]
--pop and return the front value
--@return value
function t:front()
    if self.e>self.f then
        local v= self.data[self.f]
        self.data[self.f] =nil
        self.f=self.f+1
        return v
    end  
    return nil    
end

--[Comment]
--return the number of value
--@return size
function  t:size()
    return self.e-self.f
end

--[Comment]
--is empty
--return bool
function t:is_empty()
    return self.f==self.e
end

--[Comment]
--clear the queue
function t:clear()
    while self:front() ~= nil
    do
    end
   self.f=0
   self.e=0
end


return t


