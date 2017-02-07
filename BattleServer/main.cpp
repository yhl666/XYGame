

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
 
int main11()
{
	while (1)
	{

		STARTUPINFO si;

		PROCESS_INFORMATION pi;

		ZeroMemory(&pi, sizeof(pi));

		ZeroMemory(&si, sizeof(si));

		si.cb = sizeof(si);



		CreateProcess("Server.exe", NULL, NULL, NULL, FALSE, 0, NULL, NULL, &si, &pi);
		WaitForSingleObject(pi.hProcess, INFINITE);
		Sleep(3000);
	}



	return 0;

}