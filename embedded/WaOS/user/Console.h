
#ifndef _CONSOLE
#define _CONSOLE


#define MESSAGE_OK		200
#define MESSAGE_FAIL	400
#define MESSAGE_TYPE	401


typedef struct
{
	int Sequence;
	int Type;
	int Length;
	int Data[8];
} Message;

typedef struct
{
	int Type;
	void (*Handler)(Message *);
} MessageHandler;


void Console();
int ConsoleNextInt();
void ConsoleReceiveMessage(Message *);
void ConsoleSendMessage(Message *);
void ConsoleDefaultMessageHandler(Message *);
void ConsoleMessageHelper(Message *, int, int);

#endif
