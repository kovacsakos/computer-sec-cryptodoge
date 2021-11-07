//---------------------------------------------------------------------------
#ifdef COMPILE_DLL
#include <windows.h>
//---------------------------------------------------------------------------
int WINAPI DllEntryPoint(HINSTANCE hinst, unsigned long reason, void* lpReserved)
{
    return 1;
}
//---------------------------------------------------------------------------
int WINAPI WinMain(
    HINSTANCE hInstance,
    HINSTANCE hPrevInstance,
    LPSTR lpCmdLine,
    int nCmdShow)
{
    return 0;
}
#endif
#ifdef COMPILE_EXE
#include <iostream>
#include "CaffImport.hpp"

int main() {
    std::string filename = "1.caff";
    std::ifstream caffStream(filename);
    std::stringstream buffer;
    char c;
    while (caffStream.read(&c, 1)) {
        buffer << c;
    }

    std::cout << CaffImport::importCaffAsJsonFromString(buffer.str().c_str()) << std::endl;
}
#endif
