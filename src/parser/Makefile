
SSP:=-fstack-protector-all
RELRO:=-Wl,-z,relro,-z,now
FORTIFY_SOURCE:=-D_FORTIFY_SOURCE=2

SEC_OPTIONS:=-mmitigate-rop -fstack-protector-all -fstack-clash-protection -pie -fPIE -D_FORTIFY_SOURCE=2 -fcf-protection=full

all: parser

parser: CaffImport.o Logger.o ParserException.o parser.o
	g++ $(SEC_OPTIONS) -o parser.dll parser.o CaffImport.o ParserException.o Logger.o

parser.o: parser.cpp
	g++ $(SEC_OPTIONS) -c -std=c++11 parser.cpp

CaffImport.o: CaffImport.cpp CaffImport.hpp
	g++ $(SEC_OPTIONS) -c -std=c++11 CaffImport.cpp CaffImport.hpp

Logger.o: Logger.cpp Logger.hpp
	g++ $(SEC_OPTIONS) -c -std=c++11 Logger.cpp Logger.hpp

ParserException.o: ParserException.cpp ParserException.hpp
	g++ $(SEC_OPTIONS) -c -std=c++11 ParserException.cpp ParserException.hpp

clean:
	-del parser.dll
	-del *.o

parserExeMake: parserExe

parserExe: CaffImport.o Logger.o ParserException.o parserExe.o
	g++ $(SEC_OPTIONS) -o parser.exe parserExe.o CaffImport.o ParserException.o Logger.o

parserExe.o: parserExe.cpp
	g++ $(SEC_OPTIONS) -c -std=c++11 parserExe.cpp
