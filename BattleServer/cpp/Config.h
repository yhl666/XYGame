#pragma  once
#include <string>
class Config
{
public:
	static int MAX_PLAYER_COUNT;//ÿ��������������
	static int SERVER_PORT; // �������˿�
	static int   MAX_BUFFERS11;	//socket һ��ͨ�� ��󻺳��С
	static int  GAME_OVER_FRAME_COUNT;	//���ڶ���֡ʱ ��Ϸ����  5����
	static int ROOM_UNABLE_FRAME_COUNT;	//���ڶ���֡ʱ ���䲻������Ҽ���  1����

	static bool InitWithFile(std::string file);
};




