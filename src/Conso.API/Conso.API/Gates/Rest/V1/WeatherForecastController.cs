using AutoMapper;
using Conso.API.Gates.Rest.Dto;
using Conso.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Conso.API.Gates.Rest.V1
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IServiceClass _serviceClass;
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IMapper _mapper;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };     

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IMapper mapper,
            IServiceClass serviceClass)
        {
            _logger = logger;
            _mapper = mapper;
            _serviceClass = serviceClass;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        [Authorize]
        public IEnumerable<ClassDto> Get()
        {
            var models = _serviceClass.DoGet();

            return _mapper.Map<IEnumerable<ClassDto>>(models);
        }
    }
}
