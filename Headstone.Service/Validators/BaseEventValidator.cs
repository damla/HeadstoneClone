using Headstone.Models.Events;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Service.Validator
{
    public class BaseEventValidator<T> : AbstractValidator<T> where T : BaseEvent
    {
        public BaseEventValidator()
        {
            RuleFor(x => x.Environment)
                 .NotEmpty()
                 .WithMessage("No valid environment!");

            RuleFor(x => x.UserToken)
                .NotEmpty()
                .WithMessage("No valid user token!");

            RuleFor(x => x.SessionId)
                .NotEmpty()
                .WithMessage("No valid session id!");

            RuleFor(x => x.ApplicationIP)
                .NotEmpty()
                .WithMessage("No valid application IP!");

        }

    }
}
