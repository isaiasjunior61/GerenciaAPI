using GerenciaAPI.Database;
using GerenciaAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace GerenciaAPI.src.controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DataController(AppDbContext context)
        {
            _context = context;
        }

        //Criar nova Data
        [HttpPost]
        public async Task<IActionResult> CreateData([FromBody] Data data)
        {
            _context.Datas.Add(data);
            await _context.SaveChangesAsync();
            return Ok(data);
        }

        //Buscar data por id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetData(int id)
        {
            var data = await _context.Datas.FindAsync(id);
            if (data == null)
            {
                return NotFound("Data não encontrada!");
            }
            return Ok(data);
        }

        //Atualizar data existente
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateData(int id, [FromBody] Data updatedData)
        {
            var data = await _context.Datas.FindAsync(id);
            if (data == null)
            {
                return NotFound("Data não encontrada");
            }

            data.DataInicio = updatedData.DataInicio;
            data.DataTermino = updatedData.DataTermino;
            data.Lembrete = updatedData.Lembrete;

            await _context.SaveChangesAsync();
            return Ok(data);
        }

        // Deletar uma data
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteData(int id)
        {
            var data = await _context.Datas.FindAsync(id);

            if (data == null)
                return NotFound("Data não encontrada");

            _context.Datas.Remove(data);
            await _context.SaveChangesAsync();
            return Ok("Data removida com sucesso");
        }
    }
}


