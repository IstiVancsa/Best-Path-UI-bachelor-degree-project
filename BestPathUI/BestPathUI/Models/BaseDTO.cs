using Models.DTO;
using Models.Interfaces;
using System;

namespace Models
{
    public class BaseDTO : BaseTokenizedDTO, IBaseDTO
    {
        public Guid Id { get; set; }
    }
}
