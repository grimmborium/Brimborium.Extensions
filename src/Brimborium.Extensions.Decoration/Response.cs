using System;
using System.Threading.Tasks;

namespace Brimborium.Extensions.Decoration {
    public static class Response {
        public static Response<TResult> FromResultOk<TResult>(TResult result) => new Response<TResult>(ResponseSpecification.OK, result);
        public static Response<TResult> FromResultFail<TResult>(TResult result = default) => new Response<TResult>(ResponseSpecification.Fail, result);
        public static Task<Response<TResult>> FromResultOkTask<TResult>(TResult result) => Task.FromResult(new Response<TResult>(ResponseSpecification.OK, result));
        public static Response<TResult> FromException<TResult>(System.Exception exception, TResult result = default) => new Response<TResult>(new ResponseFaulted(exception), result);
    }

    public readonly struct Response<TResult> {
        public readonly ResponseSpecification Specification { get; }
        public readonly TResult Result { get; }

        public Response(ResponseSpecification specification, TResult result) {
            this.Specification = specification;
            this.Result = result;
        }

        public void Deconstruct(out ResponseSpecification specification, out TResult result)
            => (specification, result) = (Specification, Result);

        public bool TryGetValue(out TResult result, TResult defaultValue = default) {
            if (this.Specification.IsSuccess) {
                result = this.Result;
                return true;
            } else {
                result = defaultValue;
                return false;
            }
        }

        public TResult ValueOr(TResult defaultValue = default) {
            if (this.Specification.IsSuccess) {
                return this.Result;
            } else {
                return defaultValue;
            }
        }

        public TResult ValueOr(Func<TResult> getValue) {
            if (this.Specification.IsSuccess) {
                return this.Result;
            } else {
                return getValue();
            }
        }

        public TOut Match<TOut>(Func<TResult, TOut> succ, Func<TResult, TOut> fail, Func<TResult, Exception, TOut> fault, TOut defaultValue = default) {
            var specification = this.Specification ?? ResponseSpecification.Faulted;
            if (specification.IsSuccess) {
                if (succ is null) {
                    return defaultValue;
                } else {
                    return succ(this.Result);
                }
            } else if (specification.IsFail) {
                if (fail is null) {
                    return defaultValue;
                } else {
                    return fail(this.Result);
                }
            } else if (specification.IsFaulted) {
                if (fault is null) {
                    return defaultValue;
                } else {
                    return fault(this.Result, specification.GetException()!);
                }
            }
            throw new InvalidOperationException();
        }

        public Task<TOut> MatchAsync<TOut>(Func<TResult, Task<TOut>> succ, Func<TResult, Task<TOut>> fail, Func<TResult, Exception, Task<TOut>> fault, TOut defaultValue = default) {
            var specification = this.Specification ?? ResponseSpecification.Faulted;
            if (specification.IsSuccess) {
                if (succ is null) {
                    return Task.FromResult<TOut>(defaultValue);
                } else {
                    return succ(this.Result);
                }
            } else if (specification.IsFail) {
                if (fail is null) {
                    return Task.FromResult<TOut>(defaultValue);
                } else {
                    return fail(this.Result);
                }
            } else if (specification.IsFaulted) {
                if (fault is null) {
                    return Task.FromResult<TOut>(defaultValue);
                } else {
                    return fault(this.Result, specification.GetException()!);
                }
            }
            throw new InvalidOperationException();
        }
        public async Task<Response<TOut>> ChainAsync<TOut>(Func<TResult, Task<Response<TOut>>> succ, Func<TResult, Task<Response<TOut>>> fail, Func<TResult, Exception, Task<Response<TOut>>> fault, TOut defaultValue = default) {
            var specification = this.Specification ?? ResponseSpecification.Faulted;
            if (specification.IsSuccess) {
                if (succ is null) {
                    return Response.FromResultOk(defaultValue);
                } else {
                    try {
                        return await succ(this.Result);
                        //} catch (System.AggregateException error){
                        //    return Response.FromException<TOut>(error);
                    } catch (System.Exception error) {
                        return Response.FromException<TOut>(error);
                    }
                }
            } else if (specification.IsFail) {
                if (fail is null) {
                    return Response.FromResultOk(defaultValue);
                } else {
                    try {
                        return await fail(this.Result);
                    } catch (System.Exception error) {
                        return Response.FromException<TOut>(error);
                    }
                }
            } else if (specification.IsFaulted) {
                if (fault is null) {
                    return Response.FromResultOk(defaultValue);
                } else {
                    try {
                        return await fault(this.Result, specification.GetException()!);
                    } catch (System.Exception error) {
                        return Response.FromException<TOut>(error);
                    }
                }
            }
            throw new InvalidOperationException();
        }
    }
}
