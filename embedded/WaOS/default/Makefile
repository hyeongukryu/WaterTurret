###############################################################################
# Makefile for the project WaOS
###############################################################################

## General Flags
PROJECT = WaOS
MCU = atmega128
TARGET = WaOS.elf
CC = avr-gcc

CPP = avr-g++

## Options common to compile, link and assembly rules
COMMON = -mmcu=$(MCU)

## Compile options common for all C compilation units.
CFLAGS = $(COMMON)
CFLAGS += -Wall -gdwarf-2 -std=gnu99 -DF_CPU=16000000UL -Os -funsigned-char -funsigned-bitfields -fshort-enums  -fpack-struct
CFLAGS += -MD -MP -MT $(*F).o -MF dep/$(@F).d 

## Assembly specific flags
ASMFLAGS = $(COMMON)
ASMFLAGS += $(CFLAGS)
ASMFLAGS += -x assembler-with-cpp -Wa,-gdwarf2

## Linker flags
LDFLAGS = $(COMMON)
LDFLAGS +=  -Wl,-Map=WaOS.map


## Intel Hex file production flags
HEX_FLASH_FLAGS = -R .eeprom -R .fuse -R .lock -R .signature

HEX_EEPROM_FLAGS = -j .eeprom
HEX_EEPROM_FLAGS += --set-section-flags=.eeprom="alloc,load"
HEX_EEPROM_FLAGS += --change-section-lma .eeprom=0 --no-change-warnings


## Objects that must be built in order to link
OBJECTS = waos.o waos_context.o waos_error.o waos_memory.o waos_serial.o waos_task.o waos_timer.o waos_util.o user.o Console.o waos_port.o Motor.o Water.o 

## Objects explicitly added by the user
LINKONLYOBJECTS = 

## Build
all: $(TARGET) WaOS.hex WaOS.eep WaOS.lss size

## Compile
waos.o: ../waos/waos.c
	$(CC) $(INCLUDES) $(CFLAGS) -c  $<

waos_context.o: ../waos/waos_context.c
	$(CC) $(INCLUDES) $(CFLAGS) -c  $<

waos_error.o: ../waos/waos_error.c
	$(CC) $(INCLUDES) $(CFLAGS) -c  $<

waos_memory.o: ../waos/waos_memory.c
	$(CC) $(INCLUDES) $(CFLAGS) -c  $<

waos_serial.o: ../waos/waos_serial.c
	$(CC) $(INCLUDES) $(CFLAGS) -c  $<

waos_task.o: ../waos/waos_task.c
	$(CC) $(INCLUDES) $(CFLAGS) -c  $<

waos_timer.o: ../waos/waos_timer.c
	$(CC) $(INCLUDES) $(CFLAGS) -c  $<

waos_util.o: ../waos/waos_util.c
	$(CC) $(INCLUDES) $(CFLAGS) -c  $<

user.o: ../user/user.c
	$(CC) $(INCLUDES) $(CFLAGS) -c  $<

Console.o: ../user/Console.c
	$(CC) $(INCLUDES) $(CFLAGS) -c  $<

waos_port.o: ../waos/waos_port.c
	$(CC) $(INCLUDES) $(CFLAGS) -c  $<

Motor.o: ../user/Motor.c
	$(CC) $(INCLUDES) $(CFLAGS) -c  $<

Water.o: ../user/Water.c
	$(CC) $(INCLUDES) $(CFLAGS) -c  $<

##Link
$(TARGET): $(OBJECTS)
	 $(CC) $(LDFLAGS) $(OBJECTS) $(LINKONLYOBJECTS) $(LIBDIRS) $(LIBS) -o $(TARGET)

%.hex: $(TARGET)
	avr-objcopy -O ihex $(HEX_FLASH_FLAGS)  $< $@

%.eep: $(TARGET)
	-avr-objcopy $(HEX_EEPROM_FLAGS) -O ihex $< $@ || exit 0

%.lss: $(TARGET)
	avr-objdump -h -S $< > $@

size: ${TARGET}
	@echo
	@avr-size -C --mcu=${MCU} ${TARGET}

## Clean target
.PHONY: clean
clean:
	-rm -rf $(OBJECTS) WaOS.elf dep/* WaOS.hex WaOS.eep WaOS.lss WaOS.map


## Other dependencies
-include $(shell mkdir dep 2>NUL) $(wildcard dep/*)

