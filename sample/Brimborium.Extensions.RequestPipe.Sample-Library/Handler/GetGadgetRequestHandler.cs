namespace Brimborium.Extensions.RequestPipe.Sample_Library.Handler {
    using Brimborium.Extensions.RequestPipe;
    using Brimborium.Extensions.RequestPipe.Sample_Library.Model;

    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class GetGadgetRequest : IRequest<GetGadgetResponce> {
    }

    public sealed class GetGadgetResponce {
        public List<Gadget> Value { get; set; }
    }

    public class GetGadgetRequestHandler : IRequestHandler<GetGadgetRequest, GetGadgetResponce> {

        public void SetOptions(IRequestHandlerOptions options) { }

        public Task<Response<GetGadgetResponce>> ExecuteAsync(
            GetGadgetRequest request,
            CancellationToken cancellationToken,
            IRequestHandlerExecutionContext executionContext) {
            var result = new GetGadgetResponce();
            result.Value = new List<Gadget>();
            result.Value.Add(new Gadget() { Id = 1, Name = "one" });
            result.Value.Add(new Gadget() { Id = 2, Name = "two" });
            return Response.FromResultOkTask<GetGadgetResponce>(result);
        }

    }
}
