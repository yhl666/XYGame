#pragma  once

#include <assert.h>
#include "Memory.h"
#include "Config.h"
#define  TRUE 1
#define  FALSE 0


#define  LOG_FUNC_NAME Utils::log(__FUNCTION__);

#define ZT_ASSERT(cond,what) assert(cond)

//�Ƿ������߳�
#define  ENABLE_MULTI_THREAD FALSE

//�Ƿ��� �ڴ�й¶��⣬�������߳�ʱ ��������Ч
#define  MEMORY_TRACE TRUE

//����ָ����
#define  PTR_PARAM_CHECK_RETURN(param,return_what)  if(!param)return return_what;

//һ����Ϣ���紫���β
#define  NET_MSG_ENG_LINE '^'


//һ��ͨ�����buffer size
#define  MAX_BUFFERS 2048