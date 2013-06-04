
#include "waos.h"

// 시리얼 구현
void serial_init();
int serial_tx0(char data, FILE *stream);
int serial_rx0(FILE *stream);
