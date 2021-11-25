using AutoMapper;
using CryptoDoge.BLL.Dtos;
using CryptoDoge.Model.DataTransferModels;
using CryptoDoge.Model.Entities;
using CryptoDoge.ParserService;
using System.Linq;

namespace CryptoDoge.BLL
{
	public class MapperProfiles: Profile
	{
		public MapperProfiles()
		{
			CreateMap<RegisterDto, RegisterData>();

			CreateMap<ParsedCaff, Caff>()
				.ForMember(d => d.NumberOfAnimations, s => s.MapFrom(src => src.Num_anim));

			CreateMap<string, CiffTag>().ConvertUsing<StringToCiffTagConverter>();

			CreateMap<ParsedCiff, Ciff>()
				.ForSourceMember(s => s.Pixels, o => o.DoNotValidate());

			CreateMap<Ciff, CiffDto>();

			CreateMap<Caff, CaffDto>()
				.ForMember(d => d.Captions, s => s.MapFrom(x => x.Ciffs.Select(y => y.Caption).Distinct()))
				.ForMember(d => d.Tags, s => s.MapFrom(x => x.Ciffs.SelectMany(y => y.Tags).Select(x => x.Value).Distinct()));
		}

		public class StringToCiffTagConverter : ITypeConverter<string, CiffTag>
		{
            public CiffTag Convert(string source, CiffTag destination, ResolutionContext context)
            {
				return new() { Value = source };
            }
        }
	}
}
