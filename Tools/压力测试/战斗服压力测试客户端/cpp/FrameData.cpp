
#include "FrameData.h"
using namespace std;

namespace std
{

	std::string  itos(int i)
	{
		char buff[100];
		sprintf(buff, "%d", i);
		return  std::string(buff);

	}

}
FrameData *  FrameData::CreateWithJson(const string &  json)
{
	FrameData*ret = new FrameData(json);
	if (ret)return ret;
	return  nullptr;
}

FrameData *  FrameData::CreateEmpty()
{
	FrameData*ret = new FrameData();
	if (ret)return ret;
	return  nullptr;
}


FrameData::FrameData(const string& json)
{
	this->InitWithJson(json);
}

FrameData::FrameData()
{

}

string  FrameData::toJson(bool skip)
{
	string ret;
	ret.append("no:");
	ret += std::itos(no);
	ret.append(",");

	ret.append("fps:");
	ret += std::itos(fps);
	ret.append(",");

	if ((skip  && left != 0) || !skip)
	{
		ret.append("left:");
		ret += std::itos(left);
		ret.append(",");
	}
	if ((skip  && right != 0) || !skip)
	{
		ret.append("right:");
		ret += std::itos(right);
		ret.append(",");
	}
	if ((skip  && jump != 0) || !skip)
	{
		ret.append("jump:");
		ret += std::itos(jump);
		ret.append(",");
	}
	if ((skip  && atk != 0) || !skip)
	{
		ret.append("atk:");
		ret += std::itos(atk);
		ret.append(",");
	}
	if ((skip  && s1 != 0) || !skip)
	{
		ret.append("s1:");
		ret += std::itos(s1);
		ret.append(",");
	}
	if ((skip  && stand != 0) || !skip)
	{
		ret.append("stand:");
		ret += std::itos(stand);
		ret.append(",");
	}

	return ret;
}


void  FrameData::InitWithJson(const string & json)
{
	int last = 0;
	string k, v;
	//parse kv to map
	for (int i = 0; i < json.size(); i++)
	{
		if (json[i] == ':')
		{
			k = json.substr(last, i - last);
			last = i + 1;
		}

		if (json[i] == ',')
		{
			v = json.substr(last, i - last);
			last = i + 1;
			kv[k] = v;
		}
	}
	//parse map to member data
	this->Parse();
}

void  FrameData::Parse()
{
	string & s_no = kv["no"];
	if (s_no != "")
	{
		this->no = std::stoi(s_no);
	}

	string &s_fps = kv["fps"];
	if (s_fps != "")
	{
		this->fps = std::stoi(s_fps);
	}


	{
		string &str = kv["left"];
		if (str != "")
		{
			this->left = std::stoi(str);
		}
	}

	{
		string &str = kv["right"];
		if (str != "")
		{
			this->right = std::stoi(str);
		}
	}

	{
		string &str = kv["jump"];
		if (str != "")
		{
			this->jump = std::stoi(str);
		}
	}
	{
		string &str = kv["stand"];
		if (str != "")
		{
			this->stand = std::stoi(str);
		}
	}


	{
		string &str = kv["atk"];
		if (str != "")
		{
			this->atk = std::stoi(str);
		}
	}

	{
		string &str = kv["s1"];
		if (str != "")
		{
			this->s1 = std::stoi(str);
		}
	}


}


FrameData::~FrameData()
{

}