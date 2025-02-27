using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlow.Application.Common.Exceptions;
public class NotFoundIDException : Exception
{
    public NotFoundIDException() : base("Can not found ID!") { }
}
