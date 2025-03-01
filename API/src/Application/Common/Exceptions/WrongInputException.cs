using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlow.Application.Common.Exceptions;
public class WrongInputException : Exception
{
    public WrongInputException() : base("Wrong input!") { }
    public WrongInputException(string? message)
            : base(message)
    {

    }
}
