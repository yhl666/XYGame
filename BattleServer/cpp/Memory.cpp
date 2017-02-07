
#include "Utils.h"
#include <list>
/*
#if  MEMORY_TRACE==TRUE && ENABLE_MULTI_THREAD ==FALSE11


static std::list<Memory*> objs;


void Memory::Release()
{
	delete this;
}

Memory:: ~Memory()
{
	objs.remove(this);
}

Memory::Memory()
{
	objs.push_back(this);

}

void Memory::PrintTrace()
{

	if (objs.size() == 0)
	{
		("[Memeory]: all objects are delete");
		return;
	}
	Utils::log("[Memeory]:..................................");

	for (const auto &obj : objs)
	{
		Utils::log("[Memeory]:%s object still in memeory address 0x%x", typeid(*obj).name(), obj);
	}

	Utils::log("[Memeory Track]: %d objects are alive", objs.size());
	Utils::log("[Memeory]:..................................");

}




#endif // RELEASE
*/