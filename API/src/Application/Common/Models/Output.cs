
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlow.Application.Common.Models;
public class Output
{
    public bool? isError { get; set; }=false;
    public int code { get; set; }
    public object? data { get; set; }
    public string? message { get; set; }
    public object[]? errors { get; set; }

}
