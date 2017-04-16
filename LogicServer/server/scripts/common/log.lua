local Log = {}

local log_debug = c_logger.debug
local log_info = c_logger.info
local log_warn = c_logger.warn
local log_error = c_logger.error
local log_fatal = c_logger.fatal

function Log:new(log_name)
	assert("table" == type(self))
	assert(not log_name or "string" == type(log_name))
	local log = {}
	log.log_name = log_name or "Log"
	setmetatable(log, self)
	self.__index = self
	return log
end

function Log:set_log_name(log_name)
	self.log_name = log_name
end

function Log:debug(pattern, ...)
	log_debug(self.log_name, string.format(pattern, ...))
end

function Log:info(pattern, ...)
	log_info(self.log_name, string.format(pattern, ...))
end

function Log:warn(pattern, ...)
	log_warn(self.log_name, string.format(pattern, ...))
end

function Log:error(pattern, ...)
	log_error(self.log_name, string.format(pattern, ...))
end

function Log:fatal(pattern, ...)
	log_fatal(self.log_name, string.format(pattern, ...))
end

return Log
