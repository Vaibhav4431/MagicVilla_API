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
     [Route("api/v{version:apiVersion}/StateAPI")]
    [ApiController]
    [ApiVersion("1.0" /*Deprecated = true*/)]
    public class StateAPIController : Controller
    {
        protected APIResponse _response;
        private readonly IStateRepository _dbState;
        private readonly ICountryRepository _dbCountry;
        private readonly IMapper _mapper;

        public StateAPIController(IStateRepository dbState, ICountryRepository dbCountry, IMapper mapper)
        {
            _dbState = dbState;
            _dbCountry = dbCountry;
            _mapper = mapper;
            _response = new();
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetState()
        {
            try
            {
                IEnumerable<State> StateList = await _dbState.GetAllAsync(includeProperties: "Country");
                _response.Result = _mapper.Map<List<StateDTO>>(StateList);
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

        [HttpGet("{id:int}", Name = "GetState")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StateDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetState(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var state = await _dbState.GetAsync(u => u.Id == id, includeProperties: "Country");
                if (state == null)
                {
                    return NotFound();
                }
                _response.Result = _mapper.Map<StateDTO>(state);
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
        public async Task<ActionResult<APIResponse>> CreateState([FromBody] StateCreateDTO createDTO)
        {
            try
            {
                if (await _dbState.GetAsync(u => u.StateName.ToLower() == createDTO.StateName.ToLower()) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "State already Exists!");
                    return BadRequest(ModelState);
                }
                if (await _dbCountry.GetAsync(u => u.Id == createDTO.CountryId) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Country ID is Invalid!");
                    return BadRequest(ModelState);
                }
                if (createDTO == null)
                {
                    return BadRequest(createDTO);
                }

                State state = _mapper.Map<State>(createDTO);

                await _dbState.CreateAsync(state);
                _response.Result = _mapper.Map<StateDTO>(state);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetState", new { id = state.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [HttpDelete("{id:int}", Name = "DeleteState")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteState(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var state = await _dbState.GetAsync(u => u.Id == id);
                if (state == null)
                {
                    return NotFound();
                }
                await _dbState.RemoveAsync(state);
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

        [HttpPut("{id:int}", Name = "UpdateState")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateState(int id, [FromBody] StateUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.Id)
                {
                    return BadRequest();
                }
                if (await _dbCountry.GetAsync(u => u.Id == updateDTO.CountryId) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Country ID is Invalid!");
                    return BadRequest(ModelState);
                }
                State model = _mapper.Map<State>(updateDTO);

                await _dbState.UpdateAsync(model);
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
