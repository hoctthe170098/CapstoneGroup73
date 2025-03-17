using StudyFlow.Application.Common.Exceptions;
using ValidationException = StudyFlow.Application.Common.Exceptions.ValidationException;

namespace StudyFlow.Application.Common.Behaviours;

public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
     where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v =>
                    v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Any())
            {
                if (failures[0].ErrorCode == "STT") 
                    throw new WrongInputException("Số thứ tự nội dung bài" +
                        " học phải bắt đầu từ 1 và tăng dần, không được trùng lặp.");
                else if (failures[0].ErrorCode == "Format")
                    throw new WrongInputException("Dữ liệu nhập sai format, vui lòng nhập lại");
                else throw new WrongInputException();
            }
               
        }
        return await next();
    }
}
