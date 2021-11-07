#include <windows.h>

#include <iostream>
#include "CaffImport.h"

int main() {
    std::string filename = "1.caff";
    std::ifstream caffStream(filename.c_str(), std::ios::binary);
    std::stringstream buffer;

    std::vector<unsigned char> chars;
    unsigned char c;
    while (caffStream.read(reinterpret_cast<char*>(&c), 1)) {
        chars.push_back(c);
    }

    //std::cout << CaffImport::importCaffAsJsonFromString(&chars.data()[0], chars.size()) << std::endl;
    CaffImport::importCaffAsJsonFromString(&chars.data()[0], chars.size());
}

