
#include "Player.h"
#include "Room.h"
#include "SocketClient.h"
#include "FrameData.h"
using namespace std;



void  Player::AddFrameData(FrameData*data)
{
	//PTR_PARAM_CHECK_RETURN(data);
	//由于客户端并不携带FPS 信息
	if (data->fps-1 == frames.size())
	{
		this->frames.push_back(data);
	}
	else
	{
	///	cout << "Error data" << endl;
		data->Release();
	}
}

void  Player::SetNo(int no)
{
	this->no = no;
}

int Player::GetNo()
{
	return this->no;
}


int Player::GetMaxFrameCount()
{
	return frames.size();
}

void  Player::IncreateFps(int target_fps)
{
	int delta = target_fps - this->GetMaxFrameCount();
	if (delta < 0)
	{
		//ZT_ASSERT(false, "could not less than 0");
	}
	else
	{

		while (delta-->0)
		{
			FrameData *re = FrameData::CreateEmpty();
			re->fps = target_fps;
			this->AddFrameData(re);
		}
		this->current_fps = target_fps;
	}


}

FrameData *  Player::GetFrameDataByFps(int fps)
{
	FrameData *ret = this->frames[fps - 1];

	return ret;

}



void  Player::SendJsonData(string msg)
{
	if (this->socket)
	{
		this->_sendMsg.push_back(msg);
		this->SendJsonDataHelper(current_send_ok_fps);
	}
}

bool  Player::SendJsonDataHelper(int frame)
{
	for (int i = current_send_ok_fps - 1; i < _sendMsg.size(); i++)
	{
		if (0 < socket->Send(_sendMsg[i]))//snd ok
		{
			++current_send_ok_fps;
		}
		else
		{
			return true;
		}
	}

	return true;
}



void Player::BindSocketClient(SocketClient * socket)
{
	if (this->socket == nullptr)
	{
		this->socket = socket;
		this->socket->BindEventHandler(this);
	}
	else
	{
		this->UnBindSocketClient();
		this->BindSocketClient(socket);
	}
}

void Player::UnBindSocketClient(SocketClient * socket)
{
	if (this->socket)this->socket->Release();
	this->socket = nullptr;
}

void Player::OnConnected(SocketClient *client)
{
	Utils::log("Player::On Connected");
}

void  Player::OnDisConnected(SocketClient *client)
{
	Utils::log("Player::On DisConnected");
	//	room->Remove(this);
	this->_isConnected = false;
	this->UnBindSocketClient();
}

void  Player::OnSendMessage(SocketClient *client, string what)
{


}

void  Player::OnRecvMessage(SocketClient *client, string what)
{
	//cout << "recv:" << what << endl;
	TranslateDataPack *pack = TranslateDataPack::Decode(what);

	if (pack->isCustomData)
	{
		string cmd = pack->customs[0];
		if (cmd == "chat")
		{
			room->BroadcastCustomData(what, false);
		}
		else if (cmd == "over")
		{
			this->_isOver = true;
		}
		else if (cmd == "pve")
		{
			room->BroadcastCustomData("cmd:pve");
		}
		else if (cmd == "ready")
		{
			this->isready = true;
		}
		pack->Release();
		//cout << __FUNCTION__ << what << endl;
		return;
	}

	pack->Release();
	FrameData *frame = FrameData::CreateWithJson(what);
	frame->no = this->no; // fix no
	frame->fps = current_fps + 1;

	//	cout << __FUNCTION__ << " FrameData " << frame->toJson() << endl;
	this->AddFrameData(frame);
}

void   Player::OnRecvEmptyMessage(SocketClient *client)
{
	FrameData *frame = FrameData::CreateEmpty();
	if (this->GetMaxFrameCount() > 0)
	{//预测为上个操作的dir
		FrameData* last_frame = GetFrameDataByFps(current_fps);
		frame->dir = last_frame->dir;
	}
	frame->fps = current_fps + 1;
	frame->no = this->no;
	this->AddFrameData(frame);

	//	LOG_FUNC_NAME;
}


void  Player::OnReConnected(SocketClient *client)
{


}


Player *  Player::Create(SocketClient * socket_bind, Room *room)
{
	PTR_PARAM_CHECK_RETURN(socket_bind, nullptr);

	Player *ret = new Player;
	if (!ret) return nullptr;
	ret->room = room;

	ret->BindSocketClient(socket_bind);
	return ret;
}



Player::~Player()
{
	for (auto p : frames)
	{
		p->Release();
	}
	this->UnBindSocketClient(socket);
	Utils::log("~Player FrameData Count = %d", frames.size());
}

void Player::Recv()
{
	if (this->socket)
	{
		this->socket->Recv();
	}
}

void Player::RecvTick()
{
	if (this->frames.size() == current_fps + 1)
	{
	}
	else if (this->frames.size() == current_fps)
	{
		this->Recv();
	}
	else
	{
		//ZT_ASSERT(false, "  this player frame dates Error");
	//	cout<<"this player frame dates Error   " <<current_fps<<"   "<<frames.size()<<endl;;
		//	for (int i = this->frames.size() - 1;i<;i++)
	}
}




void Player::CheckEmptyFrameTick(int fps)
{
	if (fps == 0)return;
	if (this->frames.size() == fps)
	{// Do NOT recv this frame data  ,add empty
		this->OnRecvEmptyMessage(socket);
	}
	else if (this->frames.size() == fps + 1)
	{
		//recv this frame data OK
	}
	else
	{
		//	ZT_ASSERT(false, "  this player frame dates Error");

	}

}

void Player::SendTick()
{

	/*if (this->frames.size() == current_fps + 1)
	{

	}
	else if (this->frames.size() == current_fps)
	{
	//	this->Recv();

	}
	else
	{
	ZT_ASSERT(false, "  this player frame dates Error");
	}*/
}


bool  Player::isReadyForGameOver()
{
	if (!socket)return true;
	if (isConnected() == false)return true;
	if (current_send_ok_fps == current_fps + 2)return true;// 2 is cmd(custom) counts

	if (current_fps + 1 == this->frames.size())return true;
	if (current_fps == this->frames.size())return true;

	return false;
}

bool  Player::isReady()
{
	return isready;
}