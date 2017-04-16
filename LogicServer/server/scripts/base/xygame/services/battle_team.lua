
--[[

* Author: caoshanshan
* Email: me@dreamyouxi.com
战斗 组队系统
]]


local t = { }
t.enable_hotfix = false;  -- 禁止hot fix

local log = require("log"):new("login")
local remote = require("xygame.base.remote");-- 远程服务
local team = require("model.team");
local vector = require("base.vector");

global_team_list = vector.new(); -- 全局房间信息
global_team_id_max = 0; -- 全局 队伍id最大值


-- helper function of global_team_list

-- 创建一只新队伍 仅对no 操作
local function create_new_team(captain, name)
    global_team_id_max = global_team_id_max + 1;
    local tt = team.create();
    tt.no = global_team_id_max;
    tt.captain = tonumber(captain);
    tt.captain_name = name;
    global_team_list:add(tt);
    return tt;
end

-- 向房间内 添加一个玩家  仅对no 操作
local function add_player(no, player_no)
    local team = get_team(no)

    if team == nil then
        return nil;
    end
    team.other = tonumber(player_no);
    return team;
end

-- 向房间内 一个玩家  仅对no 操作
local function remove_player(team, player_no)
    if team == nil then
        return nil;
    end
    team.other = 0;
    return team;
end


-- 查找房间
local function get_team(no)
    local team = nil;
    global_team_list:foreach( function(k, v)
        if v.no == no then
            team = v;
        end
    end )
    if team == nil then
        return nil;
    end
    return team;
end



local function remove_team(team1)
    global_team_list:remove(team1);
end;

-- 创建房间
-- 创建成功 直接返回房间no
function t.create(ctx, msg, cb)
    local kv = json.decode(msg);
    local no = kv["no"];
    local name = kv["name"];
    local teamm = create_new_team(no, name);

    if teamm == nil then
        cb("ret:error,");
    else
        cb("ret:ok,no:" .. teamm.no .. ",captain:" .. teamm.captain .. ",");
    end
end


-- 加入房间
-- 加入成功直接返回 房间no 和队长信息
function t.join(ctx, msg, cb)
    local kv = json.decode(msg);
    local no = kv["no"];
    local other = tonumber(kv["other"]);
    local name = kv["name"];
    local team = get_team(tonumber(no));
    if team == nil then
        cb("ret:error,msg:房间不存在,"); return;
    end
    if team:is_full() == true then
        cb("ret:error,msg:房间已满,"); return;
    end
    if other == team.captain then
        cb("ret:error,msg:你已经是队长无需重复加入,"); return;
    end

    local v = global_hero_list:get_hero_by_no(tostring(team.captain));
    remote.request_client(v.ctx, "TownTeam", "EnterTeam", "no:" .. other .. ",name:" .. name .. ",", function(msg)
        if msg == "timeout" then
            -- 通知失败
            remove_team(team);
            cb("ret:error,msg:通知其他玩家失败,"); return;
        else
            team.other = other;
            cb("ret:ok,no:" .. tostring(team.no) .. ",captain:" .. tostring(team.captain) .. ",name:" .. team.captain_name .. ",");
        end
    end )
end


-- 离开房间
function t.leave(ctx, msg, cb)
    local kv = json.decode(msg);
    local no = kv["no"];
    local player_no = tonumber(kv["player_no"]);

    local team = get_team(tonumber(no));
    if team == nil then
        cb("ret:error,msg:房间不存在,"); return;
    end

    if team:is_full() == true then
        if player_no == team.captain then
            -- 通知队员
            local v = global_hero_list:get_hero_by_no(tostring(team.other));
            if v == nil then
                remove_team(team);
                cb("ret:error,msg:玩家不在线,");
                return;
            end
            remote.request_client(v.ctx, "TownTeam", "LeaveTeam", "no:" .. team.captain .. ",", function(msg)
                if msg == "timeout" then
                    -- 通知失败
                    remove_team(team);
                    cb("ret:error,msg:通知其他玩家失败,");
                else
                    cb("ret:ok,msg:,");
                    team.other = 0;
                    team.captain = 0;
                    remove_team(team);
                end
            end )
            return;
        end

        if player_no == team.other then
            --    remove_player(team, team.other);
            local v = global_hero_list:get_hero_by_no(tostring(team.captain));
            if v == nil then
                remove_team(team);
                cb("ret:error,msg:玩家不在线,");
                return;
            end
            remote.request_client(v.ctx, "TownTeam", "LeaveTeam", "no:" .. team.other .. ",", function(msg)
                if msg == "timeout" then
                    -- 通知失败
                else
                    cb("ret:ok,msg:,");
                    team.other = 0;
                end
            end )
            return;
        end
    else
        cb("ret:ok,msg:,");
        remove_team(team);
    end
end

-- 搜索房间
-- 返回当前所有房间信息
function t.search(ctx, msg, cb)

    local json = "";
    --  local count = 0;
    global_team_list:foreach( function(k, v)
        --   json = json .. v.captain;
        local hero = global_hero_list:get_hero_by_no(tostring(v.captain));
        if hero ~= nil then
            json = json .. "{" .. v:to_json() .. "},";
        end
        --    json = json .. "{no:" .. v.no .. ",captain:" .. v.captain .. ",},";
        -- count = count + 1;
        --  if count >= 3 then
        -- 为了性能 限制在 只会返回部分房间
        -- return false;
        -- end
    end );

    cb(json);
end


-- 随机获取可加入的房间号
-- 随机策略 线性加入可加入的房间，返回一个房间id
function t.random(ctx, msg, cb)

    local no = -1;
    global_team_list:foreach( function(k, v)
        if v:is_full() == false then
            no = v.no;
            return false;
        end
    end );
    if no == -1 then
        cb("ret:error,msg:当前没有任何队伍!快去创建一个吧,"); return;
    end
    cb("ret:ok,no:" .. no .. ",");
end


--  
function t.change_skill(ctx, msg, cb)
    local kv = json.decode(msg);
    local no = kv["no"];
    local player_no = tonumber(kv["player_no"]);

    local team = get_team(tonumber(no));
    if team == nil then
        cb("ret:error,msg:房间不存在,"); return;
    end
    --   cb("ret:error,msg:通知其他玩家失败,");
    --  cb("ret:ok,");
    local v = nil;
    if player_no == team.captain then
        v = global_hero_list:get_hero_by_no(tostring(team.other));
    else
        v = global_hero_list:get_hero_by_no(tostring(team.captain));
    end
    if v == nil then
        remove_team(team);
        cb("ret:error,msg:玩家不在线,");
        return;
    end
    remote.request_client(v.ctx, "TownTeam", "ChangeSkill", "no:" .. player_no .. ",", function(msg)
        if msg == "timeout" then
            -- 通知失败
            cb("ret:error,msg:通知其他玩家失败,"); return;
        else
            cb("ret:ok,"); return;
        end
    end )

end


function t.change_state(ctx, msg, cb)
    local kv = json.decode(msg);
    local no = kv["no"];
    local player_no = tonumber(kv["player_no"]);

    local team = get_team(tonumber(no));
    if team == nil then
        cb("ret:error,msg:房间不存在,"); return;
    end
    --   cb("ret:error,msg:通知其他玩家失败,");
    --  cb("ret:ok,");

    local v = nil;
    if player_no == team.captain then
        v = global_hero_list:get_hero_by_no(tostring(team.other));
    else
        v = global_hero_list:get_hero_by_no(tostring(team.captain));
    end
    if v == nil then
        remove_team(team);
        cb("ret:error,msg:玩家不在线,");
        return;
    end
    remote.request_client(v.ctx, "TownTeam", "ChangeState", "no:" .. player_no .. ",", function(msg)
        if msg == "timeout" then
            -- 通知失败
            cb("ret:error,msg:通知其他玩家失败,"); return;
        else
            cb("ret:ok,"); return;
        end
    end )

end


--  
function t.start_game(ctx, msg, cb)
    local kv = json.decode(msg);
    local no = kv["no"];

    local team = get_team(tonumber(no));
    if team == nil then
        cb("ret:error,msg:房间不存在,"); return;
    end
    if team:is_full() == true then
        cb("ret:ok,");
    else
        cb("ret:error,msg:队员还没准备,"); return;
    end

    -- 开始战斗流程

    local hero_captain = global_hero_list:get_hero_by_no(tostring(team.captain));
    local hero_other = global_hero_list:get_hero_by_no(tostring(team.other));

    if hero_captain == nil then
        remove_team(team);
        cb("ret:error,msg:队长不在线,");
        return;
    end
    if hero_other == nil then
        remove_team(team);
        cb("ret:error,msg:队员不在线,");
        return;
    end


    remove_team(team);

    --  local json = "{" .. hero_captain.user.to_json() .. "},{" .. hero_other.user.to_json() .. "},";

    remote.request("services.battle_pvp", "request_pvp_v2", "no:" .. hero_captain.user.no .. ",no_target:" .. hero_other.user.no .. ",", function(msg5)

        if msg == "tiemout" then
            cb("ret:error,msg:timeout,");
            return;
        end

        msg5 = string.gsub(msg5, "ret:ok,", "");

        remote.request_client(hero_captain.ctx, "TownTeam", "StartGame", msg5 .. "mode:pve," .. hero_other.user:to_json(), function(msg)
        end )
        remote.request_client(hero_other.ctx, "TownTeam", "StartGame", msg5 .. "mode:pve," .. hero_captain.user:to_json(), function(msg)
        end )

    end );
end


return t;