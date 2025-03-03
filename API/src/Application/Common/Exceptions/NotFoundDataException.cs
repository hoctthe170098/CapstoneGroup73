using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.Common.Exceptions;
public class NotFoundDataException : Exception
{
    public NotFoundDataException() : base("Can not found data!") { }
    public NotFoundDataException(string? message)
            : base(message)
    {
        
    }
}
