using AutoMapper;
using CryptoDoge.BLL.Dtos;
using CryptoDoge.Model.DataTransferModels;
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
		}

	}
}
