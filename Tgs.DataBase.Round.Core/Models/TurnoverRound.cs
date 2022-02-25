using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.DataBase.Round.Core.Models
{
    public record TurnoverRound(
          Guid RoundId,
          decimal Turnover);
}
