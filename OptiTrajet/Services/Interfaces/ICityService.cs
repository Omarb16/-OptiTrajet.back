﻿using OptiTrajet.Dtos.Out;

namespace OptiTrajet.Services.Interfaces
{
    public interface ICityService
    {
        Task<List<CityDto>> Get();
    }
}
