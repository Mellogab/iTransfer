using iTransferencia.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTransferencia.Core.Services
{
    public interface IBacenService
    {
        public Task<HttpRequestUseCaseOutput> NotifyBacen(Transfer Transfer);
    }
}
