local t=class("Stack")
--[Comment]
--Stack
--
function t:ctor()
    self.data={}
    self.c=0
end



--[Comment]
--push value
function t:push(value)
    self.data[self.c]=value
    self.c=self.c+1
end

--[Comment]
--get stack top value
--@return value
function t:top()
    return self.data[self.c-1]
end

--[Comment]
--pop the top value
function t:pop()
    local x=self.data[self.c]
    if self.c>=1 then
        self.data[self.c]=nil
        self.c=self.c-1
        return x
    end 
    return nil     
end

--[Comment]
--return the number of value
--@return size
function  t:size()
    return self.c
end

--[Comment]
--is empty
--@return bool
function t:is_empty()
    return self.c==0
end

--[Comment]
--clear the stack
function t:clear()
    while self:isEmpty() 
    do
        self:pop()
    end
    self.c=0
end

return t
