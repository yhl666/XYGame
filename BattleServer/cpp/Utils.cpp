
#include "Utils.h"
#include <iostream>
#include "Memory.h"

using namespace  std;


std::string  Utils::itos(int i)
{
	char buff[100];
	sprintf(buff, "%d", i);
	return  std::string(buff);

}

void   Utils::log(const char *str, ...)
{
	//	return;
	//this is thread safe
	char log_buffer[1024];// 1kb   buffer size


	va_list l;
	va_start(l, str);

	vsprintf(log_buffer, str, l);

	va_end(l);
	strcat(log_buffer, "\n\0");

	//OutputDebugString(log_buffer);
	time_t rawtime;
	struct tm * t;
	time(&rawtime);
	t = localtime(&rawtime);

	printf("[%d:%d:%d]:", t->tm_hour, t->tm_min, t->tm_sec);


	printf(log_buffer);

}



static unsigned int getHashByKey(const char *key)
{
	unsigned int len = strlen(key);
	const char *end = key + len;
	unsigned int hash;

	for (hash = 0; key < end; key++)
	{
		hash *= 16777619;
		hash ^= (unsigned int)(unsigned char)toupper(*key);
	}
	return (hash);
}


