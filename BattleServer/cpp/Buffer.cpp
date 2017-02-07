
#include "Buffer.h"
using namespace  std;

/*

char *	Buffer::GetBuffer()
{
ZT_ASSERT(buffer, "non init");

return  buffer;

}

int	Buffer::GetBufferSize()
{
return  size;
}

void 	Buffer::setBufferSize(int s)
{
size = s;

}

void	Buffer::Clear()
{
if (this->buffer)
{
memset(buffer, 0, MAX_BUFFERSIZE*(sizeof(char)));
}
}

void	Buffer::Check()
{
ZT_ASSERT(buffer[MAX_BUFFERS - 1] == 0, "out of range");
}


void Buffer::Malloc()
{
if (!buffer) buffer = new char[MAX_BUFFERS];
this->Clear();
}
void Buffer::Delete()
{
if (buffer)
{
delete buffer;
this->size = 0;
}
}

Buffer::Buffer()
{
this->Malloc();
}

Buffer::Buffer(const Buffer & other)
{
this->Copy((Buffer*)&other);
}

Buffer::Buffer(const Buffer  && other)
{
this->Move((Buffer*)&other);
}

Buffer&  Buffer::  operator = (const Buffer & other)
{
this->Copy((Buffer*)&other);
return *this;
}

Buffer&  Buffer::  operator = (const Buffer && other)
{
this->Move((Buffer*)&other);
return *this;
}


void Buffer::Move(Buffer *other)
{
if (other || !other->buffer)
{
//	ZT_ASSERT(false, " other non scene");
return;
}

if (this->buffer)
{
this->Delete();
}
this->buffer = other->buffer;
this->setBufferSize(other->GetBufferSize());
other->buffer = nullptr;
other->setBufferSize(0);
}

void Buffer::Copy(Buffer * other)
{
if (other || !other->buffer)
{
ZT_ASSERT(false," other non scene");
}
if (buffer)
{
memcpy(this->buffer, other->buffer, MAX_BUFFERS);
}
else
{
this->Malloc();
this->Copy(other);
}
}

Buffer::~Buffer()
{
this->Delete();
}



*/



#include "Buffer.h"


Buffer::Buffer()
{
	this->Clear();
}


char *	Buffer::GetBuffer()
{
	return  buffer;

}

int	Buffer::GetBufferSize()
{
	return  size;
}

void 	Buffer::setBufferSize(int s)
{
	size = s;

}

void	Buffer::Clear()
{
	memset(buffer, 0, MAX_BUFFERSIZE*(sizeof(char)));
}

void	Buffer::Check()
{
	ZT_ASSERT(buffer[MAX_BUFFERS - 1] == 0, "out of range");
}







LogFile::LogFile(std::string file)
{
	file = "log/Room/" + file;
	file += ".txt";

	f.open(file, std::ios::out);
}


void  LogFile::log(const char *str, ...)
{

	char log_buffer[102400];// 1kb   buffer size


	va_list l;
	va_start(l, str);

	vsprintf(log_buffer, str, l);

	va_end(l);
	strcat(log_buffer, "\0");

	time_t rawtime;
	struct tm * t;
	time(&rawtime);
	t = localtime(&rawtime);

	char b[200];

	sprintf(b, "[%d:%d:%d]:", t->tm_hour, t->tm_min, t->tm_sec);



	f << b << log_buffer << endl;
}

LogFile::~LogFile()
{
	f.flush();
	f.close();
}