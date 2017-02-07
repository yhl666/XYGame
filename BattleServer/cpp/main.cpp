
#include <iostream>


using namespace std;




#include "ServerApp.h"
#include "Config.h"

int main()
{
	SetErrorMode(SEM_NOGPFAULTERRORBOX);//main 

	bool ok=Config::InitWithFile("config.ini");
	if (ok)
	{
		ServerAppDynamic app;
		//ServerApp app;
		app.Startup();
		app.Run();
	}
	else
	{
		cout << "config.ini  is missing or error" << endl;
	}


	system("pause");

	return 0;
}



/*
//以下是守护进程代码，崩溃自动重启，程序入口是下面代码生成的exe



#include <iostream>
#include <list>
#include <string>

#include <vector>
#include <fstream>
#include <stdlib.h>
#include <stdio.h>


#include "stdlib.h"
#include "stdio.h"
#include "string.h"
using namespace std;

#include "windows.h"

int main()
{
while(1)
{

STARTUPINFO si;

PROCESS_INFORMATION pi;

ZeroMemory( &pi, sizeof(pi) );

ZeroMemory( &si, sizeof(si) );

si.cb = sizeof(si);




CreateProcess( "Server.exe", NULL, NULL, NULL, FALSE, 0, NULL, NULL, &si, &pi );
WaitForSingleObject( pi.hProcess, INFINITE);
Sleep(3000);
}



return 0;

}

*/