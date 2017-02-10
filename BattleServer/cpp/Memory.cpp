
#include "Utils.h"
#include <list>
#include <mutex>
using namespace  std;
#ifdef  MEMORY_TRACE

static mutex locker;
#define  LOCK(what) std::lock_guard<mutex> lock(what);
static std::list<Memory*> objs;


void Memory::Release()
{
	delete this;
}

Memory:: ~Memory()
{
	LOCK(locker);
	objs.remove(this);
}

Memory::Memory()
{
	LOCK(locker);
	objs.push_back(this);

}

void Memory::PrintTrace()
{

	LOCK(locker);

	if (objs.size() == 0)
	{
		("[Memeory]: all objects are delete");
		return;
	}
	Utils::log("[Memeory]:...............start...................");
	unsigned int total = 0;
	unsigned int objsize = 0;
	for (const auto &obj : objs)
	{
		objsize = obj->GetHeapSize();
		if (objsize == 0)
		{
			objsize = sizeof(((*obj)));
		}
		Utils::log("[Memeory]:Heap: %d Bytes in 0x%x %s", objsize, obj, typeid(*obj).name());
		total += objsize;
	}

	Utils::log("[Memeory]:Heap: %d Bytes with %d objects alive.", total, objs.size());
	Utils::log("[Memeory]:................end....................");

}


void Memory::Clear()
{
	/*for (const auto &obj : objs)
	{
	obj->Release();
	}*/
	objs.clear();
}

#endif // RELEASE
