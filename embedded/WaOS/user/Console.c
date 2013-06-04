
#include "user.h"

#define MESSAGE_HANDLERS	11

MessageHandler *MessageHandlers;

void ConsoleMessageHelper(Message *m, int type, int length)
{
	m->Type = type;
	m->Length = length;
}


void Console()
{
	MessageHandlers = malloc(sizeof(MessageHandler) * MESSAGE_HANDLERS);
	
	MessageHandlers[0].Type = 0;
	MessageHandlers[0].Handler = ConsoleDefaultMessageHandler;
	
	MessageHandlers[1].Type = 20;
	MessageHandlers[1].Handler = MotorPanGet;
	MessageHandlers[2].Type = 21;
	MessageHandlers[2].Handler = MotorPanSet;

	MessageHandlers[3].Type = 30;
	MessageHandlers[3].Handler = MotorTiltNozzleGet;
	MessageHandlers[4].Type = 31;
	MessageHandlers[4].Handler = MotorTiltNozzleSet;

	MessageHandlers[5].Type = 40;
	MessageHandlers[5].Handler = MotorTiltCameraGet;
	MessageHandlers[6].Type = 41;
	MessageHandlers[6].Handler = MotorTiltCameraSet;

	MessageHandlers[7].Type = 50;
	MessageHandlers[7].Handler = WaterPumpOn;
	MessageHandlers[8].Type = 51;
	MessageHandlers[8].Handler = WaterPumpOff;
	MessageHandlers[9].Type = 52;
	MessageHandlers[9].Handler = WaterValveOn;
	MessageHandlers[10].Type = 53;
	MessageHandlers[10].Handler = WaterValveOff;



	Message *message = malloc(sizeof(Message));
	memset(message, 0x00, sizeof(Message));
	unsigned char i;

	while(1)
	{
		ConsoleReceiveMessage(message);

		for (i = 1; i < MESSAGE_HANDLERS; i++)
		{
			if (MessageHandlers[i].Type == message->Type)
			{
				UTIL_LOCK();
				MessageHandlers[i].Handler(message);
				UTIL_UNLOCK();
				break;
			}
		}

		if (i == MESSAGE_HANDLERS)
		{
			UTIL_LOCK();
			MessageHandlers[0].Handler(message);
			UTIL_UNLOCK();
		}

		ConsoleSendMessage(message);
	}
}

int ConsoleNextInt()
{
	int i;
	scanf("%d", &i);
	return i;
}

void ConsoleReceiveMessage(Message *m)
{
	m->Sequence = ConsoleNextInt();
	m->Type = ConsoleNextInt();
	m->Length = ConsoleNextInt();

	for (unsigned char i = 0; i < m->Length; i++)
	{
		m->Data[i] = ConsoleNextInt();
	}
}

void ConsoleSendMessage(Message *m)
{
	printf("%d %d %d", m->Sequence, m->Type, m->Length);
	
	for (unsigned char i = 0; i < m->Length; i++)
	{
		printf(" %d", m->Data[i]);
	}
	printf("!\n");
}


void ConsoleDefaultMessageHandler(Message *m)
{
	switch(m->Type)
	{
	case 10:
		ConsoleMessageHelper(m, MESSAGE_OK, 0);
		break;

	case 11:
		WaterEmergency();
		MotorEmergency();
		ConsoleMessageHelper(m, MESSAGE_OK, 0);
		break;

	default:
		ConsoleMessageHelper(m, MESSAGE_TYPE, 1);
		m->Data[0] = m->Type;
		break;
	}
}
