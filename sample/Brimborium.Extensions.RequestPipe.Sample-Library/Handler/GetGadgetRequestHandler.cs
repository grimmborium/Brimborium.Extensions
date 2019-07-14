namespace Brimborium.Extensions.RequestPipe.Sample_Library.Handler {
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Brimborium.Extensions.RequestPipe;
    using Brimborium.Extensions.RequestPipe.Sample_Library.Model;

    public sealed class GetGadgetRequest : IRequest<GetGadgetResponce> {
    }

    public sealed class GetGadgetResponce {
        public List<Gadget> Value { get; set; }
    }

    public class GetGadgetRequestHandler : IRequestHandler<GetGadgetRequest, GetGadgetResponce> {
        public void SetOptions(IRequestHandlerOptions options) { }

        public Task<GetGadgetResponce> ExecuteAsync(GetGadgetRequest request) {
            var result = new GetGadgetResponce();
            result.Value = new List<Gadget>();
            result.Value.Add(new Gadget() { Id = 1, Name = "one" });
            result.Value.Add(new Gadget() { Id = 2, Name = "two" });
            return Task.FromResult<GetGadgetResponce>(result);
        }
    }
}
