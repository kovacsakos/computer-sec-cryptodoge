using AutoMapper;
using CryptoDoge.BLL.Dtos;
using CryptoDoge.Model.DataTransferModels;
using CryptoDoge.Model.Entities;
using CryptoDoge.ParserService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
