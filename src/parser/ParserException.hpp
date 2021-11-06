//---------------------------------------------------------------------------
#ifndef ParseExceptionHPP
#define ParseExceptionHPP
//---------------------------------------------------------------------------
#include <string>

class ParserException {
public:
	std::string filepath;
	std::string msg;
	inline std::string what() {return msg;}
	inline ParserException(std::string what) {
		this->msg = what;
	}
};
//---------------------------------------------------------------------------
#endif