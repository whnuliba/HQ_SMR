using HPSocket;
using IDS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Device.Communication.ServerEvent
{
    public delegate IdsResult<E> ClientHandShakeEventHandler<E>(IdsUpdClient sender);
}
