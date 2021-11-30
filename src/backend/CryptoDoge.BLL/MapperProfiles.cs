﻿using AutoMapper;
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
			CreateMap<CaffComment, CaffCommentReturnDto>()
				.ForMember(d => d.UserName, s => s.MapFrom(src => src.User.UserName));

			CreateMap<ParsedCaff, Caff>()
				.ForMember(d => d.NumberOfAnimations, s => s.MapFrom(src => src.Num_anim));

			CreateMap<string, CiffTag>().ConvertUsing<StringToCiffTagConverter>();

			CreateMap<ParsedCiff, Ciff>()
				.ForSourceMember(s => s.Pixels, o => o.DoNotValidate())
				.ForMember(d => d.Tags, src => src.MapFrom(s => s.Tags.Select(t => new CiffTag() { Value = t })));

			CreateMap<Ciff, CiffDto>();

			CreateMap<Caff, CaffDto>()
				.ForMember(d => d.Captions, s => s.MapFrom(x => x.Ciffs.Select(y => y.Caption).Distinct()))
				.ForMember(d => d.Tags, s => s.MapFrom(x => x.Ciffs.SelectMany(y => y.Tags).Select(x => x.Value).Distinct()))
				.ForMember(d => d.UploadedBy, s => s.MapFrom(x => x.UploadedBy.Id))
				.ForMember(d => d.UploadedByName, s => s.MapFrom(x => x.UploadedBy.UserName));
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
