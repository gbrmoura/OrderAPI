using Microsoft.AspNetCore.Mvc.ModelBinding;
using OrderAPI.API.HTTP;
using System;
using System.Collections.Generic;

namespace OrderAPI.API.Services {

    public static class ModelStateService {

        public static List<ErrorResponse> ErrorConverter(ModelStateDictionary modelState) {
            if (modelState.ErrorCount <= 0) {
                return null;
            }

            List<ErrorResponse> httpErrorList = new List<ErrorResponse>();
            foreach (KeyValuePair<string, ModelStateEntry> model in modelState) {
                String message = "", field = model.Key;
                foreach (ModelError error in model.Value.Errors) {
                    message = error.ErrorMessage;
                }

                httpErrorList.Add(new ErrorResponse() {
                    Field = field,
                    Message = message 
                });
            }

            return httpErrorList;
        }

    }
}
