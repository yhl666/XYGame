﻿
/*

Author designed by cao shanshan
Email me@dreamyouxi.com

*/
#include <iostream>
#include <winsock2.h>
#include <LuaIntf/LuaIntf.h>
#include <assert.h>
#include <sstream>
#include <fstream>
#include <stdio.h>

#include <boost/filesystem.hpp>      //boost  

#include "log.h"
#include "XYGameLog.h"
#include "world.h"  // for GetLuaState()
#include "get_world.h"  // for GetWorld()
#include "timer_queue_root.h"
#include "timer_queue.h"
#include "..\base\ClientServer.h"


#include "windows.h"


#pragma comment(lib,"ws2_32.lib") 

XYGameLog::XYGameLog() 
{

}

void XYGameLog::Log(const std::string &file, const std::string & info)
{
	if (file.empty() == true)
	{
		std::cout << "你输入的目录为空" << std::endl;    //TODO
	}
	if (info.empty() == true)
	{
		std::cout << "输入的log信息为空" << std::endl;   //TODO
	}

	std::fstream f;
	std::string file_name;
	std::stringstream ss;

	time_t rawtime;
	struct tm * tp;
	time(&rawtime);
	tp = localtime(&rawtime);

	ss << tp->tm_year + 1900 << "-" << tp->tm_mon + 1 << "-" << tp->tm_mday << "." << "log";

	file_name = ss.str();
	//cout << file_name << endl;

	//判断目录是否存在
	boost::filesystem::path path_file(file);
	if (!(boost::filesystem::is_directory(path_file)))
	{
		this->CreateDir(file);
	}

	std::string path = file + "\\" + file_name;
	//cout << path << endl;
	f.open(path, ios::out | ios::app);
	if (f.is_open() == false)
	{
		//打开文档失败        //TODO
		return;
	}
	else
	{
		f << "[" << tp->tm_hour << ":" << tp->tm_min << ":" << tp->tm_sec << "]" << info << endl;
	}
}


void  XYGameLog::CreateDir(const std::string &file)
{
	if (file.empty() == true)
	{
		std::cout << "你输入的目录为空" << std::endl;      //TODO
	}

	assert(!file.empty());
	boost::filesystem::path path_file(file);
	if (boost::filesystem::is_directory(path_file))
	{
		//目录存在 
		return;
	}
	else
	{
		//error
		// not multi level dir
		if (std::string::npos == file.find('\\'))
		{
			return;
		}

		int i = file.size();
		for (; i >= 0; i--)
		{
			if (file[i] == '\\')
			{
				break;
			}
		}
		CreateDir(file.substr(0, i));
		boost::filesystem::create_directory(file);
	}
}
