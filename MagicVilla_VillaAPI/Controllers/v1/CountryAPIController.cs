using AutoMapper;
using MagicVilla_VillaAPI.Migrations;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers.v1
{
    [Route("api/v{version:apiVersion}/CountryAPI")]
    [ApiController]
    [ApiVersion("1.0" /*Deprecated = true*/)]
    public class CountryAPIController : Controller
    {
        protected APIResponse _response;
        private readonly ICountryRepository _dbCountry;
        private readonly IMapper _mapper;

        public CountryAPIController(ICountryRepository dbCountry, IMapper mapper)
        {
            _dbCountry = dbCountry;
            _mapper = mapper;
            _response = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetCountry()
        {
            try
            {
                IEnumerable<Country> countryList = await _dbCountry.GetAllAsync();
                _response.Result = _mapper.Map<List<CountryDTO>>(countryList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [HttpGet("{id:int}", Name = "GetCountry")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CountryDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetCountry(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var country = await _dbCountry.GetAsync(u => u.Id == id);
                if (country == null)
                {
                    return NotFound();
                }
                _response.Result = _mapper.Map<CountryDTO>(country);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateCountry([FromBody] CountryCreateDTO createDTO)
        {
            try
            {
                if (await _dbCountry.GetAsync(u => u.CountryName.ToLower() == createDTO.CountryName.ToLower()) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa already Exists!");
                    return BadRequest(ModelState);
                }
                if (createDTO == null)
                {
                    return BadRequest(createDTO);
                }

                Country country = _mapper.Map<Country>(createDTO);

                await _dbCountry.CreateAsync(country);
                _response.Result = _mapper.Map<CountryDTO>(country);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetCountry", new { id = country.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [HttpDelete("{id:int}", Name = "DeleteCountry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteCountry(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var country = await _dbCountry.GetAsync(u => u.Id == id);
                if (country == null)
                {
                    return NotFound();
                }
                await _dbCountry.RemoveAsync(country);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [HttpPut("{id:int}", Name = "UpdateCountry")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateCountry(int id, [FromBody] CountryUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.Id)
                {
                    return BadRequest();
                }
                if (await _dbCountry.GetAsync(u => u.Id == updateDTO.Id) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Country ID is Invalid!");
                    return BadRequest(ModelState);
                }
                Country model = _mapper.Map<Country>(updateDTO);

                await _dbCountry.UpdateAsync(model);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }
    }
}
