#pragma  once

#include "Defs.h"
#include <iostream>


/*
class Buffer :public Memory
{
int size = 0;

public:
Buffer();
Buffer(const Buffer & other);
Buffer(const Buffer  && other);
Buffer& operator = (const Buffer & other);
Buffer& operator = (const Buffer && other);


char *GetBuffer();

int GetBufferSize();
void setBufferSize(int s);

void Clear();
void Check();

std::string GetString(){ return buffer; }
const static int MAX_BUFFERSIZE = MAX_BUFFERS;
~Buffer();
private:
char  * buffer = 0;
void Malloc();
void Delete();
void Move(Buffer *other);
void Copy(Buffer * other);


};



*/


class Buffer :public Memory
{
	int size = 0;

public:
	Buffer();


	char *GetBuffer();

	int GetBufferSize();
	void setBufferSize(int s);

	void Clear();
	void Check();

	std::string GetString(){ return buffer; }
	const static int MAX_BUFFERSIZE = MAX_BUFFERS;
private:
	char buffer[MAX_BUFFERSIZE];



};



class LogFile : public Memory
{
public:

	LogFile(std::string file);
	void log(const char *str, ...);
	~LogFile();

private:
	std::fstream f;
};
