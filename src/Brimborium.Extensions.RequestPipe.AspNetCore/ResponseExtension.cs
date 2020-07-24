using System;

using Microsoft.AspNetCore.Mvc;

namespace Brimborium.Extensions.RequestPipe {
    public static class ResponseExtension {
        public static ActionResult<T> AsActionResult<T>(
            this Response<T> response)
            => ResponseExtension.AsActionResult<T, T>(response, (value) => value);

        public static ActionResult<TResult> AsActionResult<TResponse, TResult>(
            this Response<TResponse> response,
            Func<TResponse, TResult> getValue) {
            var specification = (response.Specification ?? ResponseSpecification.OK);
            var specificationType = specification.GetType();
            ResponseToActionResult.Get(specificationType);
            throw new Exception();
        }
    }
    public class ResponseToActionResult {
        public static void Add() { }
        public static void Get(Type specificationType) { }
    }
}
