//---------------------------------------------------------------------------
#ifndef LoggerHPP
#define LoggerHPP
//---------------------------------------------------------------------------
#include <iostream>
#include <iomanip>
#include <ctime>
#include <chrono>
#include "ParserException.hpp"

namespace Logger{
	inline void writeOne(std::string msg) {
		auto now = std::chrono::system_clock::now();
		auto t_c = std::chrono::system_clock::to_time_t(now);
		auto buf = std::localtime(&t_c);
		std::clog << (1900+buf->tm_year) << "::" 
				  << (buf->tm_mon) << "::"
				  << (buf->tm_mday) << "::"
				  << (buf->tm_hour) << "::"
				  << (buf->tm_min) << "::"
				  << (buf->tm_sec) << "::"
				  << "::" << msg << std::endl;
	}

	inline void message(std::string message) {
		writeOne(message);
	}

	inline void exception(ParserException e) {
		writeOne(std::string("EXCEPTION WHILE PARSING FILE: ") + e.filepath + std::string(e.what()));
	}
};
//---------------------------------------------------------------------------
#endif