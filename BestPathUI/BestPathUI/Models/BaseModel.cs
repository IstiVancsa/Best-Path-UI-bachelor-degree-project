using Models.Interfaces;
using System;

namespace Models
{
    public abstract class BaseModel : IBaseModel
    {
        public virtual Guid Id { get; set; }

        public IBaseDTO GetDTO()
        {
            throw new NotImplementedException();
        }
    }
}