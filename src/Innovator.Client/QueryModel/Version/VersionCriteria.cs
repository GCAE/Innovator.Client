using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innovator.Client.QueryModel
{
  public class VersionCriteria : IVersionCriteria
  {
    public IExpression Condition { get; set; }
  }
}
