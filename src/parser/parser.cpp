//---------------------------------------------------------------------------
#ifdef DO_NOT_INCLUDE
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
#endif


#include <iostream>
#include "CaffImport.hpp"

int main() {
    std::string filename = "1.caff";
    std::ifstream caffStream(filename.c_str(), std::ios::binary);
    std::stringstream buffer;

    std::vector<unsigned char> chars;
    unsigned char c;
    while (caffStream.read(reinterpret_cast<char*>(&c), 1)) {
        chars.push_back(c);
    }

    std::cout << CaffImport::importCaffAsJsonFromString(&chars.data()[0], chars.size()) << std::endl;
}

