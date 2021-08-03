using Microsoft.AspNetCore.Mvc.ModelBinding;
using OrderAPI.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.Services {
    public class ModelStateService {
        
        public ModelStateService() { }

        public static List<HttpError> ErrorConverter(ModelStateDictionary modelState) {
            if (modelState.ErrorCount <= 0) {
                return null;
            }

            List<HttpError> httpErrorList = new List<HttpError>();
            foreach (KeyValuePair<string, ModelStateEntry> model in modelState) {
                String message = "", field = model.Key;
                foreach (ModelError error in model.Value.Errors) {
                    message = error.ErrorMessage;
                }

                httpErrorList.Add(new HttpError() {
                    Field = field,
                    Message = message 
                });
            }

            return httpErrorList;
        }

    }
}
