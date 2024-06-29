using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace iTransferencia.Core.Presenters
{
    public class DefaultPresenter<T> : IOutputPort<T> where T : UseCaseResponseMessage
    {
        /// <summary>
        /// Default presenter data stored
        /// </summary>
        private T Data { get; set; }

        /// <summary>
        /// Handle Method
        /// </summary>
        /// <param name="response"></param>
        public void Handle(T response)
        {
            Data = response;
        }

        ///// <summary>
        ///// Get Result
        ///// </summary>
        ///// <returns></returns>
        //public JsonResult GetJsonResult()
        //{
        //    var result = new JsonResult(Data)
        //    {
        //        StatusCode = (int)(Data.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest)
        //    };

        //    return result;
        //}

        /// <summary>
        /// Get current presenter data
        /// </summary>
        /// <returns></returns>
        public T GetUseCaseResponse()
        {
            return Data;
        }
    }
}
