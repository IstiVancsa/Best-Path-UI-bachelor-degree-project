using System;

namespace Models.Interfaces
{
    public interface IBaseModel
    {
        Guid Id { get; set; }
        IBaseDTO GetDTO();
    }
}
