#include <iostream>
#include "CaffImport.hpp"

int main() {
    std::string filename = "1.caff";
    
    std::cout << CaffImport::importCaffAsJson(const_cast<char*>(filename.c_str())) << std::endl;
}