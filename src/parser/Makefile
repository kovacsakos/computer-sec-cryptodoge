RELRO:=-Wl,-z,relro,-z,now
FORTIFY_SOURCE:=-D_FORTIFY_SOURCE=2

SEC_OPTIONS:= -pie -fPIC
# -fstack-protector-all -fstack-clash-protection -pie -fPIE -D_FORTIFY_SOURCE=2 -fcf-protection=full -fPIC
LIB_FLAGS:= -shared -Wl,-add-stdcall-alias
ARCH_64:= -m64
SOURCE:= parser.cpp CaffImport.cpp CaffImport.h ParserException.cpp ParserException.h Logger.h Logger.cpp

dll: parser_dll
exe: parser_exe

parser_dll: 
	g++ $(SEC_OPTIONS) $(LIB_FLAGS) -o parser.dll  $(SOURCE)
parser_exe:
	g++ $(SEC_OPTIONS) -o parser.exe $(SOURCE)
clean-windows:
	-del -f parser.dll
	-del -f *.o
	-del -f parser.exe
	
clean-linux:
	-rm -f parser.exe
	-rm -f *.o