namespace Brimborium.Extensions.RequestPipe.Sample_WebAPI.Controllers {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Brimborium.Extensions.RequestPipe.Sample_Library.Handler;
    using Brimborium.Extensions.RequestPipe.Sample_Library.Model;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class GadgetController : ControllerBase {
        private readonly IRequestExecutionService _RequestExecution;

        public GadgetController(IRequestExecutionService requestExecution) {
            this._RequestExecution = requestExecution;
        }

        // GET api/Gadget
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gadget>>> Get() {
            var request = new GetGadgetRequest();
            var result = await this._RequestExecution.ExecuteAsync(request);
            return result.Value;
        }

        // GET api/Gadget/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id) {
            return "value";
        }

        // POST api/Gadget
        [HttpPost]
        public void Post([FromBody] string value) {
        }

        // PUT api/Gadget/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value) {
        }

        // DELETE api/Gadget/5
        [HttpDelete("{id}")]
        public void Delete(int id) {
        }
    }
}
