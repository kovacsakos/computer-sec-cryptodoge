#include "CaffImport.h"
#include "Logger.h"


using namespace CaffImport;

char* importCaffAsJsonFromString(uint8_t* caffBytes, uint64_t size) {

	try {
		std::vector<unsigned char> bytes;
		for (uint64_t i = 0; i < size; i++) {
			bytes.push_back(caffBytes[i]);
		}

		auto caff = importCaff(bytes);
		char* json = convertCaffToJson(caff);
		return json;
	}
	catch (ParserException& e) {
		Logger::exception(e);
		return nullptr;
	}
	catch (...) {
		return nullptr;
	}
}

char* importCaffAsJson(const char* filepath) {

	try {
		std::ifstream caffStream(filepath, std::ios::binary);

		if (!caffStream.is_open()) {
			throw ParserException("Filepath " + std::string(filepath) + " could not be opened");
		}
		std::vector<unsigned char> bytes;
		unsigned char c;
		while (caffStream.read(reinterpret_cast<char*>(&c), 1)) {
			bytes.push_back(c);
		}

		auto caff = importCaff(bytes);
		char* json = convertCaffToJson(caff);
		return json;
	}
	catch (ParserException& e) {
		Logger::exception(e);
		return nullptr;
	}
	catch (...) {
		return nullptr;
	}

}

void freeNativeMem(char* address) {
	delete[] address;
}

void appendObj(std::string& ss, std::string name, unsigned long long data) {
	ss += "\"";
	ss += name;
	ss += "\":";

	ss += std::to_string(data);
}

void appendObj(std::string& ss, std::string name, std::string data) {
	ss += "\"";
	ss += name;
	ss += "\":";

	ss += "\"";
	ss += data;
	ss += "\"";
}

char* CaffImport::convertCaffToJson(Caff& caff) {
	std::string json;
	json.reserve(200000);

	json += "{";
	appendObj(json, "num_anim", caff.num_anim);
	json += ",";
	appendObj(json, "creationYear", caff.creationYear);
	json += ",";
	appendObj(json, "creationMonth", caff.creationMonth);
	json += ",";
	appendObj(json, "creationDay", caff.creationDay);
	json += ",";
	appendObj(json, "creationHour", caff.creationHour);
	json += ",";
	appendObj(json, "creationMinute", caff.creationMinute);
	json += ",";
	appendObj(json, "creator", caff.creator);
	json += ",";

	json += "\"ciffs\":[";

	for (auto ciff : caff.ciffs) {
		json += "{";

		appendObj(json, "duration", ciff.first);
		json += ",";
		appendObj(json, "height", ciff.second.height);
		json += ",";
		appendObj(json, "width", ciff.second.width);
		json += ",";
		appendObj(json, "caption", ciff.second.caption);
		json += ",";

		json += "\"tags\":[";

		auto tagIter = ciff.second.tags.begin();
		json += "\"";
		json += *tagIter;
		json += "\"";
		for (tagIter++; tagIter != ciff.second.tags.end(); tagIter++) {
			json += ",";
			json += "\"";
			json += *tagIter;
			json += "\"";
		}
		json += "],";

		if (ciff.second.pixels.size() != 0 && ciff.second.pixels[0].size() != 0) {
			json += "\"pixels\":[";
			for (auto rowIter = ciff.second.pixels.begin(); rowIter < ciff.second.pixels.end(); rowIter++) {
				json += "[";
				for (auto colIter = rowIter->begin(); colIter != rowIter->end(); colIter++) {
					json += "[";

					json += std::to_string(colIter->r);
					json += ",";
					json += std::to_string(colIter->g);
					json += ",";
					json += std::to_string(colIter->b);

					json += "],";
				}
				json.pop_back();
				json += "],";
			}

			json.pop_back();
			json += "]},";
		}
		else
			json += "},";
	}

	json.pop_back();
	json += "]}";

	char* retArray = new char[json.length() + 1];
	retArray[json.length()] = '\0';
	unsigned long long retArrIdx = 0;
	for (auto jsonIter = json.begin(); jsonIter < json.end(); jsonIter++, retArrIdx++) {
		retArray[retArrIdx] = *jsonIter;
	}
	return retArray;
}

Caff CaffImport::importCaff(std::vector<unsigned char>& bytes) {
	Logger::message("Reading caff from vector");
	auto caffBlocks = readCaffBlocks(bytes);
	return parseCaffBlocks(caffBlocks);

}


CaffBlocks CaffImport::readCaffBlocks(std::vector<unsigned char>& bytes) {
	CaffBlocks newCaff;


	auto length = bytes.size();
	if (length > pow(2, sizeof(void*) * 8) - 1) {
		throw ParserException("System architecture doesn't support caff size of " + length);
	}

	auto begin = bytes.begin();
	auto end = bytes.end();
	newCaff = getBlocks(begin, end);

	return newCaff;
}

template<typename Iterator, is_forward_iterator<Iterator>>
CaffBlocks CaffImport::getBlocks(Iterator& begin, Iterator& end) {
	CaffBlocks rawCaff;
	rawCaff.getBlocks().emplace_back(Block(begin, end));
	auto header = rawCaff.getBlocks()[0];
	auto headerBegin = header.begin();
	for (short i = 0; i < 12; i++) headerBegin++;

	unsigned char buffer[8];
	for (short i = 0; i < 8; i++) buffer[i] = *headerBegin++;
	uint64_t supposedNumAnim = *(reinterpret_cast<uint64_t*>(buffer));
	std::stringstream numAnimStream;
	numAnimStream << supposedNumAnim;

	while (begin != end) {
		if (supposedNumAnim == rawCaff.getBlocks().size() - 2)
			throw ParserException("Read " + numAnimStream.str() + " blocks, but there are bytes remaining");
		rawCaff.getBlocks().emplace_back(Block{ begin, end });
	}
	return rawCaff;
}

template <typename Iterator, is_forward_iterator<Iterator>>
Block::Block(Iterator& begin, Iterator& end) {
	id = std::make_shared<unsigned char>();
	*id = *begin++;
	short idInt = (int)*id;
	if (idInt != 1 && idInt != 2 && idInt != 3)
		throw ParserException("Id must be 1, 2 or 3.");

	unsigned char buffer[8];

	for (short i = 0; i < 8; i++) buffer[i] = *begin++;
	length = *reinterpret_cast<uint64_t*>(buffer);

	auto actLength = end - begin;
	if (length > actLength)
		throw ParserException("Length is bigger than actual length of iterators' difference");
	data.resize(length);

	for (uint64_t i = 0; i < length; i++) {
		data[i] = *begin++;
	}
}

CaffImport::Caff CaffImport::parseCaffBlocks(CaffBlocks rc) {
	Caff caff;

	auto caffHeader = rc[0];
	parseCaffHeader(caffHeader, caff);

	auto caffCredits = rc[1];
	parseCaffCredits(caffCredits, caff);

	for (uint64_t i = 2; i < rc.size(); i++) {
		auto nextCiff = parseCaffAnimBlock(rc[i]);
		caff.ciffs.push_back(std::move(nextCiff));
	}
	return caff;
}

void CaffImport::parseCaffHeader(Block& header, Caff& caff) {

	if ((int)header.getId() != 1) throw ParserException("Id of header is not 1");

	auto headerIter = header.begin();
	uint8_t C = *headerIter++;
	uint8_t A = *headerIter++;
	uint8_t F1 = *headerIter++;
	uint8_t F2 = *headerIter++;
	if (C != 'C' || A != 'A' || F1 != 'F' || F2 != 'F') throw ParserException("CAFF magic doesn't match");

	uint64_t length = readLong(headerIter);	//HEADER_SIZE
	if (length != header.getLength()) throw ParserException("Lengths of header block do not match.");
	caff.num_anim = readLong(headerIter);	//NUM_ANIM
}

void CaffImport::parseCaffCredits(Block& credits, Caff& caff) {

	if ((int)credits.getId() != 2) throw ParserException("Id of credits is not 2");

	auto creditsIter = credits.begin();
	unsigned char yearBuf[2];
	yearBuf[0] = *creditsIter++;
	yearBuf[1] = *creditsIter++;
	caff.creationYear = *reinterpret_cast<uint16_t*>(&yearBuf[0]);
	caff.creationMonth = (short)*creditsIter++;
	caff.creationDay = (short)*creditsIter++;
	caff.creationHour = (short)*creditsIter++;
	caff.creationMinute = (short)*creditsIter++;

	uint64_t creatorLength = readLong(creditsIter);
	std::string creator;
	creator.resize(creatorLength);
	for (uint64_t i = 0; i < creatorLength; i++) creator[i] = *creditsIter++;
	caff.creator = std::move(creator);
}

template <typename Iterator, is_forward_iterator<Iterator>>
uint64_t CaffImport::readLong(Iterator& iterator) {
	unsigned char buffer[8];
	for (int i = 0; i < 8; i++, iterator++) buffer[i] = *iterator;
	uint64_t* value;
	value = reinterpret_cast<uint64_t*>(&buffer[0]);
	return *value;
}

std::pair<duration_t, Ciff> CaffImport::parseCaffAnimBlock(Block& caffAnimBlock) {
	if ((int)caffAnimBlock.getId() != 3) throw ParserException("Id of caff anim block is not 3");

	auto animIter = caffAnimBlock.begin();
	auto animIterEnd = caffAnimBlock.end();

	uint64_t duration = readLong(animIter);

	Ciff parsedCiff = parseCiff(animIter, animIterEnd);

	return std::pair<duration_t, Ciff>{ duration, parsedCiff };
}

template<typename Iterator, is_forward_iterator<Iterator>>
Ciff CaffImport::parseCiff(Iterator& ciffIter, Iterator& ciffIterEnd) {

	uint8_t C = *ciffIter++;
	uint8_t I = *ciffIter++;
	uint8_t F1 = *ciffIter++;
	uint8_t F2 = *ciffIter++;
	if (C != 'C' || I != 'I' || F1 != 'F' || F2 != 'F') throw ParserException("CIFF magic doesn't match");

	unsigned long long headerSize = readLong(ciffIter);
	unsigned long long contentSize = readLong(ciffIter);
	unsigned long long width = readLong(ciffIter);
	unsigned long long height = readLong(ciffIter);
	bool noPixel = false;
	if (width == 0 || height == 0) noPixel = true;
	if (contentSize != width * height * 3) throw ParserException("Content size doesn't match width*height*3");

	std::string caption;
	while (*ciffIter != '\n') caption.push_back((char)*ciffIter++);
	ciffIter++;

	std::vector<std::string> tags;
	while (std::ptrdiff_t(ciffIterEnd - ciffIter) > contentSize) {
		std::string currentTag;
		while (*ciffIter != '\0') {
			if (*ciffIter == '\n') throw ParserException("Tags cannot be multiline.");
			currentTag.push_back((char)*ciffIter++);
		}
		ciffIter++;
		tags.push_back(std::move(currentTag));
	}

	Ciff newCiff{ height, width };
	newCiff.caption = std::move(caption);
	newCiff.tags = std::move(tags);
	if (noPixel) {
		if (ciffIter != ciffIterEnd) throw ParserException("Ciff should not contain pixels according to header, but pixels are present");
		return newCiff;
	}
	for (unsigned long long i = 0; i < height; i++) {
		for (unsigned int j = 0; j < width; j++) {
			newCiff.pixels[i][j].r = *ciffIter++;
			newCiff.pixels[i][j].g = *ciffIter++;
			newCiff.pixels[i][j].b = *ciffIter++;
		}
	}
	if (ciffIter != ciffIterEnd) throw ParserException("Error after parsing ciff, begin and end iterators do not match");
	return newCiff;
}

Ciff::Ciff(uint64_t height, uint64_t width) {
	reSize(height, width);
}

void Ciff::reSize(uint64_t height, uint64_t width) {
	pixels.resize(height);
	for (std::vector<Pixel>& row : pixels) row.resize(width);
	this->width = width;
	this->height = height;
}
