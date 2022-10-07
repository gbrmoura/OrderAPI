using Microsoft.AspNetCore.Mvc.ModelBinding;
using OrderAPI.Domain.Http;
using System;
using System.Collections.Generic;

namespace OrderAPI.API.Services {

    public class ModelService {

        public List<ErrorResponse> ErrorConverter(ModelStateDictionary model) 
        {
            if (model.ErrorCount <= 0) 
            {
                return null;
            }

            List<ErrorResponse> errors = new List<ErrorResponse>();
            foreach (var key in model) 
            {
                string message = String.Empty; 
                string field = key.Key;
                foreach (ModelError error in key.Value.Errors) 
                {
                    message = error.ErrorMessage;
                }

                errors.Add(new ErrorResponse() 
                {
                    Field = field,
                    Message = message 
                });
            }

            return errors;
        }

    }
}
