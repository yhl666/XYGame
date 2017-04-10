#pragma  once
#include "Defs.h"
#include <iostream>
#include <unordered_map>
#include <string>
#include <vector>

/**
 * @brief  a data structure of frame data for client and server
 */
class FrameData : public Memory
{

public:
	int fps = 0;//帧数信息
	int no = 0;//该房间的玩家编号

	// 按钮状态，可用int的位 来表示，暂时不优化

	int left = 0;//left
	int right = 0;//right
	int jump = 0;//jump
	int atk = 0;//atk
	int s1 = 0;//skill 1
	int stand = 0;//stand
	int revive = 0;//revive point
	int dir = -1;
	int opt = 0;
public:

	/**
	 * @brief  create a frame data from json
	 * @param see InitWithJson(...)
	 */
	static FrameData *CreateWithJson(const std::string &  json);

	static FrameData *CreateEmpty();



	FrameData(const  std::string& json);
	FrameData();

	/**
	* @brief convert Frame to json
	* @return  string,a json
	*/
	std::string toJson(bool skip = true);

	/**
	 * @brief  init this with json
	 * @param json json must be a simple json such op:1,data:2,
	 * @note json must have a , append json
	 * //必须是简单的JSON 而且末端多一个逗号
	 */
	void InitWithJson(const  std::string & json);

	~FrameData();
private:

	/**
	 * @brief  parse k-v to mclass ember from map
	 */
	void Parse();
	std::unordered_map< std::string, std::string> kv;

};






class TranslateDataPack :public Memory
{
public:
	bool isCustomData = true;
	std::string data;//for frame data
	std::vector<std::string> customs; // for custom data


	static  TranslateDataPack* Decode(std::string msg)
	{
		TranslateDataPack *ret = new TranslateDataPack;

		if (msg == "")
		{
			ret->Release();
			return nullptr;
		}
		if (msg.size() < 4)
		{
			ret->Release();
			return nullptr;
		}// illegal 

		if (msg.substr(0, 3) == "cmd")
		{
			int last = 4;
			ret->isCustomData = true;
			for (int i = 4; i < msg.size(); i++)
			{
				char ch = msg[i];
				if (ch == ':')
				{
					ret->customs.push_back(msg.substr(last, i - last));
					last = i + 1;
				}
			}
			ret->customs.push_back(msg.substr(last));
		}
		else
		{
			ret->isCustomData = false;
			ret->data = msg;
		}
		return ret;
	}
};




