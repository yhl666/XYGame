#pragma  once
#include "Defs.h"
#include "Utils.h"

#if    ENABLE_MULTI_THREAD == TRUE &&  MEMORY_TRACE == TRUE  //&& ENABLE_MULTI_THREAD ==FALSE

class Memory
{
public:
	void Release();
	virtual  ~Memory();
	Memory();
	static  void PrintTrace();

};



#else

class Memory
{
public:
	void Release()
	{
		delete this;
	}
	virtual  ~Memory(){}
	Memory(){}
	static  void PrintTrace(){}

};

class CloneAble
{
public:
	virtual void Clone() = 0;
};
#endif // _DEBUG





