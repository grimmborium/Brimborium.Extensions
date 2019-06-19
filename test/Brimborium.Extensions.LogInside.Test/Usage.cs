using System;
using Xunit;

namespace Brimborium.Extensions.LogInside.Test {
    public class Usage {
        private string Message;

        private void Log(string message) {
            this.Message = message;
        }

        [Fact]
        public void Usage1() {

            var x = new InsideLog();
            for (int i = 0; i < 10; i++) {
                var y = x.Log(i.ToString());
                try {
                    if (i == 4) {
                        throw new Exception("four");
                    }
                } catch (Exception error){
                    this.Log($"{error.Message} {y}");
                }
            }
            Assert.Equal("four 4", this.Message);
            //var value1 = getRandom();
            //var value2 = getRandom();
            //var condition1 = x.Condition(
            //    ((value1 % 2) == 0), 
            //    "condition1");
            //var condition2 = x.Condition(
            //    (value1, value2),
            //    (a) => (((a.value1+a.value2) % 2) == 0),
            //    "condition2");
            //try {
            //    if (condition1) {
            //        throw new Exception("T");
            //    } else {
            //        throw new Exception("F");
            //    }
            //} catch {
            //}
            //if (condition2) {
            //}
        }

        private int getRandom() {
            return (new Random()).Next();
        }
    }
}
