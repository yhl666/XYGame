#pragma  once
#include "Defs.h"
#include "Utils.h"

#ifndef MEMORY_TRACE

class Memory
{
public:
	void Release(){ delete this; }
	virtual  ~Memory(){}
	Memory(){}
	virtual int GetHeapSize(){ return 0; }
	static  void PrintTrace(){};
	static void Clear(){};
};



#else

class Memory
{
public:
	void Release();
	virtual  ~Memory();
	Memory();
	virtual int GetHeapSize(){ return 0; }
	static  void PrintTrace();
	static void Clear();

};


#endif // _DEBUG



class CloneAble
{
public:
	virtual void Clone() = 0;
};