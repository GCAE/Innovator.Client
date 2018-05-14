using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innovator.Client.QueryModel
{
  internal class SqlNode
  {
    public virtual int StartOffset { get; set; }
    public SqlType Type { get; set; }

    public SqlNode()
    {
      this.StartOffset = -1;
    }

    public override int GetHashCode()
    {
      return this.StartOffset;
    }

    public override bool Equals(object obj)
    {
      var node = obj as SqlNode;
      if (node == null) return false;
      return Equals(node);
    }

    public virtual bool Equals(SqlNode obj)
    {
      return this.StartOffset == obj.StartOffset;
    }
  }
}
