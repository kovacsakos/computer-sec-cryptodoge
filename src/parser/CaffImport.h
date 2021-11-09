//---------------------------------------------------------------------------
#ifndef CaffImportHPP
#define CaffImportHPP
//---------------------------------------------------------------------------
#ifdef WINDOWS_BUILD
#include <windows.h>
#endif
#include <fstream>
#include <vector>
#include <memory>
#include <type_traits>
#include <iterator>
#include <functional>
#include <map>
#include <sstream>
#include <string>
#include <cmath>
#include <iostream>

#ifdef __cplusplus
extern "C"
{
#endif

	__declspec(dllexport) char* __stdcall importCaffAsJsonFromString(uint8_t* caffBytes, uint64_t size);
	__declspec(dllexport) char* __stdcall importCaffAsJson(const char* filepath);
	__declspec(dllexport) void __stdcall freeNativeMem(char* address);

#ifdef __cplusplus
}
#endif


namespace CaffImport {

	template <typename Iterator>
	using is_forward_iterator = typename std::enable_if<
		std::is_base_of<
		std::forward_iterator_tag,
		typename std::iterator_traits<Iterator>::iterator_category>::value
	>::type*;


	using duration_t = uint64_t;

	class Block {
	private:
		std::shared_ptr<unsigned char> id;
		//length need not be stored, because after reading the files, it will be stored implicitly by data
		std::vector<unsigned char> data;
		//uint64_t is guaranteed to be 64 bits long by cpp specification
		uint64_t length;
	public:

		template <typename Iterator, is_forward_iterator<Iterator> = nullptr>
		Block(Iterator& begin, Iterator& end);

		inline unsigned char getId() { return *id; }
		inline uint64_t getLength() { return data.size(); }
		inline unsigned char operator[](uint64_t idx) { return data[idx]; }
		inline auto begin() -> decltype(data.begin()) { return data.begin(); }
		inline auto end() -> decltype(data.end()) { return data.end(); }
	};

	class CaffBlocks {
	public:
		std::vector<Block> blocks;

		friend std::istream;

		inline Block& operator[](uint64_t idx) { return blocks[idx]; }
		inline std::vector<Block>& getBlocks() { return blocks; }
		inline uint64_t size() { return blocks.size(); }

		inline CaffBlocks() { blocks = std::vector<Block>(); }
		inline CaffBlocks(const CaffBlocks& other) {
			blocks = other.blocks;
		}
		inline CaffBlocks& operator=(CaffBlocks&& other) noexcept {
			blocks = std::move(other.blocks);
			return *this;
		}
		inline CaffBlocks& operator=(const CaffBlocks& other) {
			blocks = other.blocks;
			return *this;
		}
	};

	class Pixel {
	public:
		unsigned char r, g, b;
		inline Pixel() { r = g = b = '\0'; }
	};

	class Ciff {
	public:
		//CIFF HEADER
		uint64_t height;
		uint64_t width;
		std::string caption;
		std::vector<std::string> tags;
		//CIFF PIXELS
		std::vector<std::vector<Pixel>> pixels;

		inline Ciff() {
			height = 0;
			width = 0;
		}
		Ciff(uint64_t height, uint64_t width);
		void reSize(uint64_t height, uint64_t width);

	};

	class Caff {
	public:
		//CAFF HEADER
		uint64_t num_anim;

		//CAFF CREDITS
		short creationYear;
		short creationMonth;
		short creationDay;
		short creationHour;
		short creationMinute;
		std::string creator;

		std::vector<std::pair<duration_t, Ciff>> ciffs;
	};

	char* convertCaffToJson(Caff& caff);

	CaffBlocks readCaffBlocks(std::vector<unsigned char>& bytes);

	template <typename Iterator, is_forward_iterator<Iterator> = nullptr>
	CaffBlocks getBlocks(Iterator& begin, Iterator& end);

	template <typename Iterator, is_forward_iterator<Iterator> = nullptr>
	uint64_t readLong(Iterator& iterator);

	void parseCaffHeader(Block& header, Caff& caff);

	void parseCaffCredits(Block& credits, Caff& caff);

	template<typename Iterator, is_forward_iterator<Iterator> = nullptr>
	Ciff parseCiff(Iterator& ciffIter, Iterator& ciffIterEnd);

	std::pair<duration_t, Ciff> parseCaffAnimBlock(Block& caffAnimBlock);
	Caff parseCaffBlocks(CaffBlocks rc);

	Caff importCaff(std::vector<unsigned char>& bytes);

};

//---------------------------------------------------------------------------
#endif
