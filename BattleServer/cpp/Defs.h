#pragma  once

#include <assert.h>



#define  LOG_FUNC_NAME Utils::log(__FUNCTION__);

#define ZT_ASSERT(cond,what) assert(cond)

//�Ƿ��� �ڴ�й¶��⣬
#define  MEMORY_TRACE 

//����ָ����
#define  PTR_PARAM_CHECK_RETURN(param,return_what)  if(!param)return return_what;

//һ����Ϣ���紫���β
#define  NET_MSG_ENG_LINE '^'


//һ��ͨ�����buffer size
#define  MAX_BUFFERS 2048

#include "Config.h"
#include "Memory.h"