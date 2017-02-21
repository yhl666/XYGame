--[[
热更新辅助。利用lfs检测文件更新。
--]]

local M = { }

local lfs = require("lfs")
local hotfix = require("hotfix")
local Log = require("log")
local log = Log:new(...)
local timer_queue = c_timer_queue.CTimerQueue()

local path_to_time = { }

local global_objects = {
    arg,
    assert,
    bit32,
    collectgarbage,
    coroutine,
    debug,
    dofile,
    error,
    getmetatable,
    io,
    ipairs,
    lfs,
    load,
    loadfile,
    loadstring,
    math,
    module,
    next,
    os,
    package,
    pairs,
    pcall,
    print,
    rawequal,
    rawget,
    rawlen,
    rawset,
    require,
    select,
    setmetatable,
    string,
    table,
    tonumber,
    tostring,
    type,
    unpack,
    utf8,
    xpcall,
}

-- 允许手动check(). 因为test_client没有定时器，只能手动check().
function M.check()
	-- log:debug("check()")
	local MOD_NAME = "hotfix_module_names"
	if not package.searchpath(MOD_NAME, package.path) then return end
	package.loaded[MOD_NAME] = nil
	local module_names = require(MOD_NAME)

	for _, module_name in pairs(module_names) do
		local path, err = package.searchpath(module_name, package.path)
		-- Skip non-exist module.
		if not path then
			log:warn("No such module: %s. %s", module_name, err)
			goto continue
		end

		local file_time = lfs.attributes (path, "modification")
		if file_time == path_to_time[path] then goto continue end

		log:info("Hot fix module %s (%s)", module_name, path)
		path_to_time[path] = file_time
		hotfix.hotfix_module(module_name)
		::continue::
	end  -- for




    for _, module_name in pairs(M.names) do
		local path, err = package.searchpath(module_name, package.path)
		-- Skip non-exist module.
		if not path then
			log:warn("No such module: %s. %s", module_name, err)
			goto continue
		end

		local file_time = lfs.attributes (path, "modification")
		if file_time == path_to_time[path] then goto continue end

		log:info("Hot fix module %s (%s)", module_name, path)
		path_to_time[path] = file_time
		hotfix.hotfix_module(module_name)
		::continue::
	end  -- for


end  -- check()

function M.init()
	log:debug("init()")
	local log_hotfix = Log:new("hotfix")
	hotfix.log_debug = function(s) log_hotfix:debug(s) end
	hotfix.add_protect(global_objects)

	-- 每3s检查一次
	timer_queue:insert_repeat_from_now(1.0, 3.0, function() M.check() end)
end

function M.add_module_name(file)
    table.insert(M.names,file);
end

M.names={};
return M
