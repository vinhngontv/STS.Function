using System;
using Microsoft.Azure.WebJobs;

namespace Ordering.API.Services;

public interface IExecutionContext
{
    Guid InvocationId { get; set; }
    string FunctionName { get; set; }
    string FunctionDirectory { get; set; }
    string FunctionAppDirectory { get; set; }
}

public class FuncExecutionContext :
    ExecutionContext, IExecutionContext
{
        
}