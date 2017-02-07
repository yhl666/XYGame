#pragma  once

#include <iostream>
#include <string>
#include <vector>
#include <stdlib.h>
#include <stdio.h>
#include "stdlib.h"
#include "stdio.h"
#include "time.h"
#include <fstream>
#include "windows.h"
#include "Defs.h"
#include <iostream>




/**
 * @brief  some useful static function
 */

class  Utils
{
public:
	/**
	 * @brief log mssage
	 */
	static void log(const char *str, ...);

	/**
	 * @brief  int to string
	 */
	static std::string  itos(int i);
	static int stoi(const std::string & s)
	{
		return std::stoi(s);
	}

};









