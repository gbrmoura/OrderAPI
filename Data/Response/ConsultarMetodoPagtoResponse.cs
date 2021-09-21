using OrderAPI.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Data.Response
{

  public class ConsultarMetodoPagtoResponse
  {
    [Required(ErrorMessage = "Nome deve ser informado.")]
    [MaxLength(45, ErrorMessage = "O limite de 45 caractéres foi atigido.")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "Status deve ser informado.")]
    public Boolean Status { get; set; }

  }
}
