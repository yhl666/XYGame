
--[[
��ʼ��һЩģ���
* Author: caoshanshan
* Email: me@dreamyouxi.com

---------------------------------------------
API˵����
    pack��Ϊ��json  ��protobuf�Ĺ��ߺ�����Э��ת����Ҫ�ã����Ƿ���㲻��Ҫ���ø�ģ��


]]


require("xygame.lib.extern");
require("xygame.lib.json");

local pb = require("protobuf")

pack = { }
-- json string to pb
function pack.encode(msg)
    return pb.encode("rpc.EnterRoomMsg", { peer_name = msg });
end

-- pb to json string
function pack.decode(content)
    return(pb.decode("rpc.EnterRoomMsg", content))["peer_name"];
end




print("lib init ok");