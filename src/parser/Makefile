
SSP:=-fstack-protector-all
RELRO:=-Wl,-z,relro,-z,now
FORTIFY_SOURCE:=-D_FORTIFY_SOURCE=2

SEC_OPTIONS:= -fstack-protector-all -fstack-clash-protection -pie -fPIE -D_FORTIFY_SOURCE=2 -fcf-protection=full
ARCH_64:= -m64

dll: parser_dll
exe: parser_exe

parser_dll: CaffImport_dll.o Logger.o ParserException.o parser_dll.o
	g++ $(SEC_OPTIONS) $(ARCH_64) -o parser.dll parser.o CaffImport.o ParserException.o Logger.o

parser_exe: CaffImport_exe.o Logger.o ParserException.o parser_exe.o
	g++ $(SEC_OPTIONS) $(ARCH_64) -o parser.exe parser.o CaffImport.o ParserException.o Logger.o

parser_dll.o: parser.cpp
	g++ $(SEC_OPTIONS) $(ARCH_64) -D COMPILE_DLL -c -std=c++11 parser.cpp
	
parser_exe.o: parser.cpp
	g++ $(SEC_OPTIONS) $(ARCH_64) -D COMPILE_EXE -c -std=c++11 parser.cpp
	
CaffImport_dll.o: CaffImport.cpp CaffImport.hpp
	g++ $(SEC_OPTIONS) $(ARCH_64) -D COMPILE_DLL -c -std=c++11 CaffImport.cpp CaffImport.hpp


CaffImport_exe.o: CaffImport.cpp CaffImport.hpp
	g++ $(SEC_OPTIONS) $(ARCH_64) -D COMPILE_EXE -c -std=c++11 CaffImport.cpp CaffImport.hpp


Logger.o: Logger.cpp Logger.hpp
	g++ $(SEC_OPTIONS) $(ARCH_64) -c -std=c++11 Logger.cpp Logger.hpp

ParserException.o: ParserException.cpp ParserException.hpp
	g++ $(SEC_OPTIONS) $(ARCH_64) -c -std=c++11 ParserException.cpp ParserException.hpp

clean-windows:
	-del -f parser.dll
	-del -f *.o
	-del -f parser.exe
	
clean-linux:
	-rm -f parser.exe
	-rm -f *.o
	
