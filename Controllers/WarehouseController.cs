using Tutorial9.Exceptions;
using Tutorial9.Modeks;
using Tutorial9.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tutorial9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WareHouseController : ControllerBase
    {
        private readonly IwareHouseService _service;

        public WareHouseController(IwareHouseService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] WarehouseRequest warehouseRequest)
        {
            if (warehouseRequest == null)
            {
                return BadRequest("niepoprawne");
            }

            
            
            try
            {
                int a = await _service.add(warehouseRequest);
                
                return Ok(a);
            }
            catch (ConflictException ex) 
            {
               return Conflict(ex.Message);
            }
            catch (NotFoundException ex) 
            {
               return NotFound(ex.Message);
            }
          
        }

        [HttpPost("procedure")]
        public async Task<IActionResult> AddProcedure([FromBody] WarehouseRequest warehouseRequest)
        {
            try
            {
                await _service.ProcedureAsync(warehouseRequest.IdProduct, warehouseRequest.IdWarehouse,
                    warehouseRequest.Amount, warehouseRequest.CreatedAt);
                return Ok(warehouseRequest.IdProduct);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }
    }
}
