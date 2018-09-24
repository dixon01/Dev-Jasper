namespace JsonMGITester.JsonRpc
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public class RpcRequest
    {
        public RpcRequest()
        {
            this.@params = new List<string>();
        }

        // ReSharper disable InconsistentNaming
        [SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "JSON-RPC name")]
        public string jsonrpc { get; set; }

        [SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "JSON-RPC name")]
        public string id { get; set; }

        [SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "JSON-RPC name")]
        public string method { get; set; }

        [SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "JSON-RPC name")]
        public List<string> @params { get; set; }

        // ReSharper restore InconsistentNaming
    }
}
